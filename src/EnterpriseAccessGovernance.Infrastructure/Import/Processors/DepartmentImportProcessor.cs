using EnterpriseAccessGovernance.Application.Common.Interfaces;
using EnterpriseAccessGovernance.Application.Common.Models;
using EnterpriseAccessGovernance.Domain.Entities;

namespace EnterpriseAccessGovernance.Infrastructure.Import.Processors;

public sealed class DepartmentImportProcessor
    : IImportDatasetProcessor
{
    private readonly IImportRepository _importRepository;
    private readonly IImportRowValidator _rowValidator;

    public DepartmentImportProcessor(
        IImportRepository importRepository,
        IImportRowValidator rowValidator)
    {
        _importRepository = importRepository;
        _rowValidator = rowValidator;
    }

    public DatasetType DatasetType =>
        DatasetType.Departments;

    public async Task<ImportProcessingResult> ProcessAsync(
        IReadOnlyCollection<CanonicalImportRow> rows,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(rows);

        var successfulRecords = 0;

        var errors =
            new List<ImportProcessingError>();

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
                        DatasetType.Departments);

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

                var departmentName =
                    row.Get(ImportField.Department)!;

                var departmentCode =
                    row.Get(ImportField.DepartmentCode)!;

                var existingDepartment =
                    await _importRepository
                        .GetDepartmentByNameAsync(
                            departmentName,
                            cancellationToken);

                if (existingDepartment is null)
                {
                    var existingDepartmentByCode =
                        await _importRepository
                            .GetDepartmentByCodeAsync(
                                departmentCode,
                                cancellationToken);

                    if (existingDepartmentByCode is not null)
                    {
                        errors.Add(
                            new ImportProcessingError
                            {
                                RowNumber = rowNumber,
                                ErrorMessage =
                                    $"Department code '{departmentCode}' " +
                                    "already belongs to another department."
                            });

                        continue;
                    }

                    var department =
                        Department.Create(
                            departmentName,
                            departmentCode);

                    await _importRepository
                        .AddDepartmentAsync(
                            department,
                            cancellationToken);
                }

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

        await _importRepository
            .SaveChangesAsync(
                cancellationToken);

        if (errors.Count == 0)
        {
            return ImportProcessingResult.Success();
        }

        return ImportProcessingResult.PartialFailure(errors);
    }
}