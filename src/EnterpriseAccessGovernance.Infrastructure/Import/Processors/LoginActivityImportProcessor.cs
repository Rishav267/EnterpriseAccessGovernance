using System.Globalization;
using EnterpriseAccessGovernance.Application.Common.Interfaces;
using EnterpriseAccessGovernance.Application.Common.Models;
using EnterpriseAccessGovernance.Domain.Entities;

namespace EnterpriseAccessGovernance.Infrastructure.Import.Processors;

public sealed class LoginActivityImportProcessor
    : IImportDatasetProcessor
{
    private readonly IImportRepository _importRepository;
    private readonly IImportRowValidator _rowValidator;

    public LoginActivityImportProcessor(
        IImportRepository importRepository,
        IImportRowValidator rowValidator)
    {
        _importRepository = importRepository
            ?? throw new ArgumentNullException(nameof(importRepository));

        _rowValidator = rowValidator
            ?? throw new ArgumentNullException(nameof(rowValidator));
    }

    public DatasetType DatasetType =>
        DatasetType.LoginActivity;

    public async Task<ImportProcessingResult> ProcessAsync(
        IReadOnlyCollection<CanonicalImportRow> rows,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(rows);

        var errors = new List<ImportProcessingError>();

        foreach (var row in rows)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var validationResult =
                    _rowValidator.Validate(
                        row,
                        DatasetType.LoginActivity);

                if (!validationResult.IsValid)
                {
                    errors.Add(
                        new ImportProcessingError
                        {
                            ErrorMessage = string.Join(
                                " ",
                                validationResult.Errors)
                        });

                    continue;
                }

                var employeeNumber =
                    row.Get(ImportField.EmployeeId);

                if (string.IsNullOrWhiteSpace(employeeNumber))
                {
                    errors.Add(
                        new ImportProcessingError
                        {
                            ErrorMessage =
                                "EmployeeId is required for login activity import."
                        });

                    continue;
                }

                var employee =
                    await _importRepository
                        .GetEmployeeByEmployeeNumberAsync(
                            employeeNumber,
                            cancellationToken);

                if (employee is null)
                {
                    errors.Add(
                        new ImportProcessingError
                        {
                            ErrorMessage =
                                $"Employee '{employeeNumber}' does not exist."
                        });

                    continue;
                }

                var loginDateValue =
                    row.Get(ImportField.LastLoginDate);

                if (!DateTime.TryParse(
                        loginDateValue,
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.AssumeUniversal |
                        DateTimeStyles.AdjustToUniversal,
                        out var loginAtUtc))
                {
                    errors.Add(
                        new ImportProcessingError
                        {
                            ErrorMessage =
                                "LastLoginDate is not a valid date."
                        });

                    continue;
                }

                var loginActivity =
                    LoginActivity.Create(
                        employee.Id,
                        loginAtUtc);

                await _importRepository
                    .AddLoginActivityAsync(
                        loginActivity,
                        cancellationToken);
            }
            catch (Exception exception)
            {
                errors.Add(
                    new ImportProcessingError
                    {
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
}