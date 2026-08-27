using System.Globalization;
using EnterpriseAccessGovernance.Application.Common.Interfaces;
using EnterpriseAccessGovernance.Application.Common.Models;
using EnterpriseAccessGovernance.Domain.Entities;

namespace EnterpriseAccessGovernance.Infrastructure.Import.Processors;

public sealed class AccessAssignmentImportProcessor
    : IImportDatasetProcessor
{
    private readonly IImportRepository _importRepository;

    public AccessAssignmentImportProcessor(
        IImportRepository importRepository)
    {
        _importRepository =
            importRepository
            ?? throw new ArgumentNullException(
                nameof(importRepository));
    }

    public DatasetType DatasetType =>
        DatasetType.AccessAssignments;

    public async Task<ImportProcessingResult> ProcessAsync(
        IReadOnlyCollection<CanonicalImportRow> rows,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(rows);

        var errors =
            new List<ImportProcessingError>();

        // Tracks assignments created during this import operation.
        // Prevents duplicate rows in the same file from creating
        // duplicate database records.
        var processedAssignments =
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

                employeeNumber = employeeNumber.Trim();

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
                // Application
                // -------------------------------------------------

                var applicationName =
                    row.Get(ImportField.ApplicationName);

                if (string.IsNullOrWhiteSpace(applicationName))
                {
                    errors.Add(
                        CreateError(
                            rowNumber,
                            "ApplicationName is required."));

                    continue;
                }

                applicationName = applicationName.Trim();

                var application =
                    await _importRepository
                        .GetApplicationByNameAsync(
                            applicationName,
                            cancellationToken);

                if (application is null)
                {
                    errors.Add(
                        CreateError(
                            rowNumber,
                            $"Application '{applicationName}' does not exist."));

                    continue;
                }

                // -------------------------------------------------
                // Role
                // -------------------------------------------------

                var roleName =
                    row.Get(ImportField.RoleName);

                if (string.IsNullOrWhiteSpace(roleName))
                {
                    errors.Add(
                        CreateError(
                            rowNumber,
                            "RoleName is required."));

                    continue;
                }

                roleName = roleName.Trim();

                var role =
                    await _importRepository
                        .GetRoleByNameAsync(
                            application.Id,
                            roleName,
                            cancellationToken);

                if (role is null)
                {
                    errors.Add(
                        CreateError(
                            rowNumber,
                            $"Role '{roleName}' does not exist " +
                            $"for application '{applicationName}'."));

                    continue;
                }

                // -------------------------------------------------
                // Duplicate detection within current import
                // -------------------------------------------------

                var assignmentKey =
                    $"{employee.Id:N}:" +
                    $"{application.Id:N}:" +
                    $"{role.Id:N}";

                if (!processedAssignments.Add(assignmentKey))
                {
                    // Same assignment appeared earlier in this file.
                    // Treat it as idempotent rather than creating
                    // another database record.
                    continue;
                }

                // -------------------------------------------------
                // Access dates
                // -------------------------------------------------

                var grantedAtUtc =
                    ParseDate(
                        row.Get(ImportField.AccessStartDate))
                    ?? DateTime.UtcNow;

                var expiresAtUtc =
                    ParseDate(
                        row.Get(ImportField.AccessEndDate));

                if (expiresAtUtc.HasValue &&
                    expiresAtUtc.Value <= grantedAtUtc)
                {
                    errors.Add(
                        CreateError(
                            rowNumber,
                            "AccessEndDate must be after AccessStartDate."));

                    continue;
                }

                // -------------------------------------------------
                // Existing assignment in database
                // -------------------------------------------------

                var existingAssignment =
                    await _importRepository
                        .GetAccessAssignmentAsync(
                            employee.Id,
                            application.Id,
                            role.Id,
                            cancellationToken);

                if (existingAssignment is not null)
                {
                    // Already exists in database.
                    // Import remains idempotent.
                    continue;
                }

                // -------------------------------------------------
                // Create assignment
                // -------------------------------------------------

                var accessAssignment =
                    AccessAssignment.Create(
                        employee.Id,
                        application.Id,
                        role.Id,
                        grantedAtUtc,
                        expiresAtUtc);

                // -------------------------------------------------
                // Apply imported status
                // -------------------------------------------------

                var accessStatus =
                    row.Get(ImportField.AccessStatus);

                if (!string.IsNullOrWhiteSpace(accessStatus))
                {
                    ApplyStatus(
                        accessAssignment,
                        accessStatus.Trim(),
                        grantedAtUtc);
                }

                await _importRepository
                    .AddAccessAssignmentAsync(
                        accessAssignment,
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

        // SaveChanges is intentionally NOT called here.
        //
        // ImportService owns the overall import transaction/persistence
        // boundary and calls SaveChangesAsync after processing.
        //
        // This keeps the processor consistent with the other processors.

        if (errors.Count > 0)
        {
            return ImportProcessingResult.PartialFailure(
                errors);
        }

        return ImportProcessingResult.Success();
    }

    private static void ApplyStatus(
        AccessAssignment accessAssignment,
        string status,
        DateTime currentUtc)
    {
        if (status.Equals(
                "Active",
                StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        if (status.Equals(
                "PendingReview",
                StringComparison.OrdinalIgnoreCase))
        {
            accessAssignment.StartCertification();
            return;
        }

        if (status.Equals(
                "Revoked",
                StringComparison.OrdinalIgnoreCase))
        {
            accessAssignment.Revoke(currentUtc);
            return;
        }

        throw new InvalidOperationException(
            $"Unsupported access status '{status}'.");
    }

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