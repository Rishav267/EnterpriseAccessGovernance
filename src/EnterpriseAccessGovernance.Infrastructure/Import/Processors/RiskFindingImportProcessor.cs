using System.Globalization;
using EnterpriseAccessGovernance.Application.Common.Interfaces;
using EnterpriseAccessGovernance.Application.Common.Models;
using EnterpriseAccessGovernance.Domain.Entities;
using EnterpriseAccessGovernance.Domain.Enums;

namespace EnterpriseAccessGovernance.Infrastructure.Import.Processors;

public sealed class RiskFindingImportProcessor
    : IImportDatasetProcessor
{
    private readonly IImportRepository _importRepository;

    public RiskFindingImportProcessor(
        IImportRepository importRepository)
    {
        _importRepository =
            importRepository
            ?? throw new ArgumentNullException(
                nameof(importRepository));
    }

    public DatasetType DatasetType =>
        DatasetType.RiskFindings;

    public async Task<ImportProcessingResult> ProcessAsync(
        IReadOnlyCollection<CanonicalImportRow> rows,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(rows);

        var errors =
            new List<ImportProcessingError>();

        var processedFindings =
            new HashSet<string>(
                StringComparer.OrdinalIgnoreCase);

        var rowNumber = 1;

        foreach (var row in rows)
        {
            cancellationToken.ThrowIfCancellationRequested();

            rowNumber++;

            try
            {
                // -------------------------------------------------
                // Employee
                // -------------------------------------------------

                var employeeNumber =
                    row.Get(ImportField.EmployeeId);

                if (string.IsNullOrWhiteSpace(employeeNumber))
                {
                    errors.Add(
                        CreateError(
                            rowNumber,
                            "EmployeeId is required."));

                    continue;
                }

                employeeNumber =
                    employeeNumber.Trim();

                var employee =
                    await _importRepository
                        .GetEmployeeByEmployeeNumberAsync(
                            employeeNumber,
                            cancellationToken);

                if (employee is null)
                {
                    errors.Add(
                        CreateError(
                            rowNumber,
                            $"Employee '{employeeNumber}' does not exist."));

                    continue;
                }

                // -------------------------------------------------
                // Rule Code
                // -------------------------------------------------

                var ruleCode =
                    row.Get(ImportField.RuleCode);

                if (string.IsNullOrWhiteSpace(ruleCode))
                {
                    errors.Add(
                        CreateError(
                            rowNumber,
                            "RuleCode is required."));

                    continue;
                }

                ruleCode =
                    ruleCode.Trim().ToUpperInvariant();

                // -------------------------------------------------
                // Description
                // -------------------------------------------------

                var description =
                    row.Get(ImportField.Description);

                if (string.IsNullOrWhiteSpace(description))
                {
                    errors.Add(
                        CreateError(
                            rowNumber,
                            "Description is required."));

                    continue;
                }

                description =
                    description.Trim();

                // -------------------------------------------------
                // Severity
                // -------------------------------------------------

                var severityValue =
                    row.Get(ImportField.RiskSeverity);

                if (string.IsNullOrWhiteSpace(severityValue))
                {
                    errors.Add(
                        CreateError(
                            rowNumber,
                            "RiskSeverity is required."));

                    continue;
                }

                if (!Enum.TryParse<RiskSeverity>(
                        severityValue.Trim(),
                        true,
                        out var severity))
                {
                    errors.Add(
                        CreateError(
                            rowNumber,
                            $"Invalid risk severity '{severityValue}'. " +
                            "Expected Low, Medium, High or Critical."));

                    continue;
                }

                // -------------------------------------------------
                // Duplicate detection within current import
                // -------------------------------------------------

                var findingKey =
                    $"{employee.Id:N}:{ruleCode}";

                if (!processedFindings.Add(findingKey))
                {
                    // Same employee + rule already appeared in
                    // this import file.
                    continue;
                }

                // -------------------------------------------------
                // Existing finding in database
                // -------------------------------------------------

                var existingFinding =
                    await _importRepository
                        .GetRiskFindingAsync(
                            employee.Id,
                            ruleCode,
                            cancellationToken);

                if (existingFinding is not null)
                {
                    // Import is idempotent.
                    continue;
                }

                // -------------------------------------------------
                // Detected date
                // -------------------------------------------------

                var detectedAtUtc =
                    ParseDate(
                        row.Get(ImportField.DetectedAtUtc))
                    ?? DateTime.UtcNow;

                // -------------------------------------------------
                // Create Risk Finding
                // -------------------------------------------------

                var riskFinding =
                    RiskFinding.Create(
                        employee.Id,
                        ruleCode,
                        description,
                        severity,
                        detectedAtUtc);

                // -------------------------------------------------
                // Persist
                // -------------------------------------------------

                await _importRepository
                    .AddRiskFindingAsync(
                        riskFinding,
                        cancellationToken);
            }
            catch (Exception exception)
            {
                errors.Add(
                    CreateError(
                        rowNumber,
                        exception.Message));
            }
        }

        // ImportService owns SaveChangesAsync.
        //
        // This processor only adds entities to the DbContext.

        if (errors.Count > 0)
        {
            return ImportProcessingResult.PartialFailure(
                errors);
        }

        return ImportProcessingResult.Success();
    }

    // =============================================================
    // Date parsing
    // =============================================================

    private static DateTime? ParseDate(
        string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (!DateTime.TryParse(
                value,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal |
                DateTimeStyles.AdjustToUniversal,
                out var parsedDate))
        {
            throw new InvalidOperationException(
                $"'{value}' is not a valid date.");
        }

        return parsedDate;
    }

    // =============================================================
    // Error
    // =============================================================

    private static ImportProcessingError CreateError(
        int rowNumber,
        string message)
    {
        return new ImportProcessingError
        {
            RowNumber = rowNumber,
            ErrorMessage = message
        };
    }
}