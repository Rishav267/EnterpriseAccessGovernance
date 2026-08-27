using EnterpriseAccessGovernance.Application.Common.Interfaces;
using EnterpriseAccessGovernance.Application.Common.Models;
using EnterpriseAccessGovernance.Domain.Entities;

namespace EnterpriseAccessGovernance.Infrastructure.Import.Processors;

public sealed class ApplicationImportProcessor
    : IImportDatasetProcessor
{
    private readonly IImportRepository _importRepository;
    private readonly IImportRowValidator _rowValidator;

    public ApplicationImportProcessor(
        IImportRepository importRepository,
        IImportRowValidator rowValidator)
    {
        _importRepository = importRepository;
        _rowValidator = rowValidator;
    }

    public DatasetType DatasetType =>
        DatasetType.Applications;

    public async Task<ImportProcessingResult> ProcessAsync(
        IReadOnlyCollection<CanonicalImportRow> rows,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(rows);

        var successfulRecords = 0;

        var errors = new List<ImportProcessingError>();

        var rowNumber = 1;

        foreach (var row in rows)
        {
            cancellationToken.ThrowIfCancellationRequested();

            rowNumber++;

            try
            {
                var validationResult =
                    _rowValidator.Validate(
                        row,
                        DatasetType.Applications);

                if (!validationResult.IsValid)
                {
                    errors.Add(
                        new ImportProcessingError
                        {
                            RowNumber = rowNumber,
                            ErrorMessage = string.Join(
                                " ",
                                validationResult.Errors)
                        });

                    continue;
                }

                var applicationName =
                    row.Get(ImportField.ApplicationName)!;

                var existingApplication =
                    await _importRepository
                        .GetApplicationByNameAsync(
                            applicationName,
                            cancellationToken);

                if (existingApplication is not null)
                {
                    successfulRecords++;
                    continue;
                }

                var applicationCode =
                    row.Get(ImportField.ApplicationId);

                if (string.IsNullOrWhiteSpace(applicationCode))
                {
                    applicationCode =
                        CreateApplicationCode(applicationName);
                }

                var existingApplicationByCode =
                    await _importRepository
                        .GetApplicationByCodeAsync(
                            applicationCode,
                            cancellationToken);

                if (existingApplicationByCode is not null)
                {
                    errors.Add(
                        new ImportProcessingError
                        {
                            RowNumber = rowNumber,
                            ErrorMessage =
                                $"Application code '{applicationCode}' " +
                                "already belongs to another application."
                        });

                    continue;
                }

                var application =
                    EnterpriseApplication.Create(
                        applicationName,
                        applicationCode);

                await _importRepository
                    .AddApplicationAsync(
                        application,
                        cancellationToken);

                successfulRecords++;
            }
            catch (Exception exception)
            {
                errors.Add(
                    new ImportProcessingError
                    {
                        RowNumber = rowNumber,
                        ErrorMessage = exception.Message
                    });
            }
        }

        await _importRepository.SaveChangesAsync(
            cancellationToken);

        if (errors.Count == 0)
        {
            return ImportProcessingResult.Success();
        }

        return ImportProcessingResult.PartialFailure(errors);
    }

    private static string CreateApplicationCode(
        string applicationName)
    {
        var words =
            applicationName
                .Split(
                    ' ',
                    StringSplitOptions.RemoveEmptyEntries |
                    StringSplitOptions.TrimEntries);

        var code =
            string.Concat(
                words.Select(
                    word => char.ToUpperInvariant(
                        word[0])));

        if (string.IsNullOrWhiteSpace(code))
        {
            throw new InvalidOperationException(
                "Unable to generate application code.");
        }

        return code;
    }
}