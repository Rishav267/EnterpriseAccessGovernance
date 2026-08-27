using EnterpriseAccessGovernance.Application.Common.Interfaces;
using EnterpriseAccessGovernance.Application.Common.Models;
using EnterpriseAccessGovernance.Application.Features.Imports.DTOs;
using EnterpriseAccessGovernance.Application.Features.Imports.Interfaces;
using EnterpriseAccessGovernance.Domain.Entities;

namespace EnterpriseAccessGovernance.Application.Features.Imports.Services;

public sealed class ImportService : IImportService
{
    private readonly IEnumerable<IImportFileReader> _fileReaders;
    private readonly IImportHeaderMapper _headerMapper;
    private readonly IImportRowMapper _rowMapper;
    private readonly IDatasetDetector _datasetDetector;
    private readonly IImportRowValidator _rowValidator;
    private readonly IEnumerable<IImportDatasetProcessor> _processors;
    private readonly IImportRepository _importRepository;

    public ImportService(
        IEnumerable<IImportFileReader> fileReaders,
        IImportHeaderMapper headerMapper,
        IImportRowMapper rowMapper,
        IDatasetDetector datasetDetector,
        IImportRowValidator rowValidator,
        IEnumerable<IImportDatasetProcessor> processors,
        IImportRepository importRepository)
    {
        _fileReaders = fileReaders
            ?? throw new ArgumentNullException(nameof(fileReaders));

        _headerMapper = headerMapper
            ?? throw new ArgumentNullException(nameof(headerMapper));

        _rowMapper = rowMapper
            ?? throw new ArgumentNullException(nameof(rowMapper));

        _datasetDetector = datasetDetector
            ?? throw new ArgumentNullException(nameof(datasetDetector));

        _rowValidator = rowValidator
            ?? throw new ArgumentNullException(nameof(rowValidator));

        _processors = processors
            ?? throw new ArgumentNullException(nameof(processors));

        _importRepository = importRepository
            ?? throw new ArgumentNullException(nameof(importRepository));
    }

    public async Task<ImportResponseDto> ImportAsync(
        Stream fileStream,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(fileStream);

        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException(
                "File name is required.",
                nameof(fileName));
        }

        cancellationToken.ThrowIfCancellationRequested();

        var fileExtension = Path.GetExtension(fileName);

        if (string.IsNullOrWhiteSpace(fileExtension))
        {
            throw new ArgumentException(
                "The uploaded file must have a valid extension.",
                nameof(fileName));
        }

        var fileReader = _fileReaders.FirstOrDefault(
            reader => reader.CanRead(fileExtension));

        if (fileReader is null)
        {
            throw new InvalidOperationException(
                $"Unsupported file type '{fileExtension}'. " +
                "Only CSV and XLSX files are supported.");
        }

        var importBatch = ImportBatch.Create(
            fileName,
            fileExtension,
            DateTime.UtcNow);

        await _importRepository.AddBatchAsync(
            importBatch,
            cancellationToken);

        try
        {
            // ---------------------------------------------------------
            // 1. Read the uploaded file
            // ---------------------------------------------------------

            var fileData = await fileReader.ReadAsync(
                fileStream,
                cancellationToken);

            // ---------------------------------------------------------
            // 2. Validate that the file contains headers
            // ---------------------------------------------------------

            if (fileData.Headers.Count == 0)
            {
                importBatch.StartProcessing(0);
                importBatch.Fail(DateTime.UtcNow);

                var error = "The uploaded file does not contain any headers.";

                await _importRepository.SaveChangesAsync(
                    cancellationToken);

                return BuildResponse(
                    importBatch,
                    [error]);
            }

            // ---------------------------------------------------------
            // 3. Map source headers to canonical fields
            // ---------------------------------------------------------

            var mappingResult = _headerMapper.Map(
                fileData.Headers);

            if (!mappingResult.IsValid)
            {
                return await HandleMappingFailureAsync(
                    importBatch,
                    fileData.Rows.Count,
                    mappingResult,
                    cancellationToken);
            }

            // ---------------------------------------------------------
            // 4. Detect the dataset type
            // ---------------------------------------------------------

            var mappedFields = mappingResult.Mappings
                .Select(x => x.Field)
                .ToHashSet();

            var detectionResult = _datasetDetector.Detect(
                mappedFields);

            if (!detectionResult.IsDetected)
            {
                return await HandleDatasetDetectionFailureAsync(
                    importBatch,
                    fileData.Rows.Count,
                    detectionResult,
                    cancellationToken);
            }

            // ---------------------------------------------------------
            // 5. Find processor for detected dataset
            // ---------------------------------------------------------

            var processor = _processors.FirstOrDefault(
                x => x.DatasetType == detectionResult.DatasetType);

            if (processor is null)
            {
                throw new InvalidOperationException(
                    $"No processor is registered for dataset type " +
                    $"'{detectionResult.DatasetType}'.");
            }

            // ---------------------------------------------------------
            // 6. Map source rows to canonical rows
            // ---------------------------------------------------------

            var canonicalRows = fileData.Rows
                .Select(row => _rowMapper.Map(
                    row,
                    mappingResult.Mappings))
                .ToList();

            importBatch.StartProcessing(
                canonicalRows.Count);

            var errors = new List<string>();

            // ---------------------------------------------------------
            // 7. Validate and process every row
            // ---------------------------------------------------------

            for (var index = 0;
                 index < canonicalRows.Count;
                 index++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                // Header is row 1, therefore data starts at row 2.
                var rowNumber = index + 2;

                var row = canonicalRows[index];

                // -----------------------------------------------------
                // Row validation
                // -----------------------------------------------------

                var validationResult = _rowValidator.Validate(
                    row,
                    detectionResult.DatasetType);

                if (!validationResult.IsValid)
                {
                    importBatch.RecordFailure();

                    foreach (var validationError
                             in validationResult.Errors)
                    {
                        var errorMessage =
                            $"Row {rowNumber}: {validationError}";

                        errors.Add(errorMessage);

                        await AddImportErrorAsync(
                            importBatch,
                            rowNumber,
                            validationError);
                    }

                    continue;
                }

                // -----------------------------------------------------
                // Dataset-specific processing
                // -----------------------------------------------------

                var processingResult =
                    await processor.ProcessAsync(
                        [row],
                        cancellationToken);

                if (processingResult.IsSuccess)
                {
                    importBatch.RecordSuccess();
                }
                else
                {
                    importBatch.RecordFailure();

                    if (processingResult.Errors.Count == 0)
                    {
                        const string defaultError =
                            "Unable to process the row.";

                        errors.Add(
                            $"Row {rowNumber}: {defaultError}");

                        await AddImportErrorAsync(
                            importBatch,
                            rowNumber,
                            defaultError);
                    }
                    else
                    {
                        foreach (var processingError in processingResult.Errors)
                        {
                            var errorMessage =
                                $"Row {rowNumber}: " +
                                processingError.ErrorMessage;

                            errors.Add(errorMessage);

                            await AddImportErrorAsync(
                                importBatch,
                                rowNumber,
                                processingError.ErrorMessage);
                        }
                    }
                }
            }

            // ---------------------------------------------------------
            // 8. Complete the import
            // ---------------------------------------------------------

            importBatch.Complete(DateTime.UtcNow);

            await _importRepository.SaveChangesAsync(
                cancellationToken);

            return BuildResponse(
                importBatch,
                errors);
        }
        catch (OperationCanceledException)
        {
            // Do not convert cancellation into a normal import failure.
            // The request itself has been cancelled.
            throw;
        }
        catch
        {
            importBatch.Fail(DateTime.UtcNow);

            await _importRepository.SaveChangesAsync(
                cancellationToken);

            throw;
        }
    }

    // =================================================================
    // Mapping failure
    // =================================================================

    private async Task<ImportResponseDto>
        HandleMappingFailureAsync(
            ImportBatch importBatch,
            int totalRecords,
            ImportMappingResult mappingResult,
            CancellationToken cancellationToken)
    {
        importBatch.StartProcessing(totalRecords);

        var errors = BuildMappingErrors(mappingResult);

        // Every data row is considered failed because the file
        // structure itself is invalid.
        for (var index = 0; index < totalRecords; index++)
        {
            importBatch.RecordFailure();
        }

        // Store mapping errors.
        //
        // These errors describe the file rather than individual rows,
        // therefore row number 1 is used.
        foreach (var error in errors)
        {
            await AddImportErrorAsync(
                importBatch,
                1,
                error);
        }

        importBatch.Fail(DateTime.UtcNow);

        await _importRepository.SaveChangesAsync(
            cancellationToken);

        return BuildResponse(
            importBatch,
            errors);
    }

    // =================================================================
    // Dataset detection failure
    // =================================================================

    private async Task<ImportResponseDto>
        HandleDatasetDetectionFailureAsync(
            ImportBatch importBatch,
            int totalRecords,
            DatasetDetectionResult detectionResult,
            CancellationToken cancellationToken)
    {
        importBatch.StartProcessing(totalRecords);

        var errorMessage =
            detectionResult.ErrorMessage
            ?? "Unable to determine the dataset type.";

        // The file cannot be processed because the dataset type
        // cannot be identified.
        for (var index = 0; index < totalRecords; index++)
        {
            importBatch.RecordFailure();
        }

        await AddImportErrorAsync(
            importBatch,
            1,
            errorMessage);

        importBatch.Fail(DateTime.UtcNow);

        await _importRepository.SaveChangesAsync(
            cancellationToken);

        return BuildResponse(
            importBatch,
            [errorMessage]);
    }

    // =================================================================
    // Add import error
    // =================================================================

    private async Task AddImportErrorAsync(
        ImportBatch importBatch,
        int rowNumber,
        string errorMessage)
    {
        var importError = ImportError.Create(
            importBatch.Id,
            rowNumber,
            errorMessage);

        await _importRepository.AddErrorAsync(
            importError);
    }

    // =================================================================
    // Build mapping errors
    // =================================================================

    private static IReadOnlyCollection<string>
        BuildMappingErrors(
            ImportMappingResult mappingResult)
    {
        var errors = new List<string>();

        foreach (var field
                 in mappingResult.MissingRequiredFields)
        {
            errors.Add(
                $"Required field '{field}' is missing.");
        }

        foreach (var header
                 in mappingResult.AmbiguousHeaders)
        {
            errors.Add(
                $"Header '{header}' is ambiguous or duplicated.");
        }

        foreach (var header
                 in mappingResult.UnknownHeaders)
        {
            errors.Add(
                $"Unknown header '{header}'.");
        }

        return errors;
    }

    // =================================================================
    // Build response
    // =================================================================

    private static ImportResponseDto BuildResponse(
        ImportBatch importBatch,
        IReadOnlyCollection<string> errors)
    {
        return new ImportResponseDto
        {
            ImportBatchId = importBatch.Id,

            Status = importBatch.Status.ToString(),

            TotalRecords =
                importBatch.TotalRecords,

            SuccessfulRecords =
                importBatch.SuccessfullyProcessedRecords,

            FailedRecords =
                importBatch.FailedRecords,

            Errors = errors
        };
    }
}