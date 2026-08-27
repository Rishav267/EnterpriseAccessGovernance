using EnterpriseAccessGovernance.Application.Common.Interfaces;
using EnterpriseAccessGovernance.Application.Common.Models;
using EnterpriseAccessGovernance.Domain.Entities;

namespace EnterpriseAccessGovernance.Infrastructure.Import.Processors;

public sealed class RolePermissionImportProcessor
    : IImportDatasetProcessor
{
    private readonly IImportRepository _importRepository;
    private readonly IImportRowValidator _rowValidator;

    public RolePermissionImportProcessor(
        IImportRepository importRepository,
        IImportRowValidator rowValidator)
    {
        _importRepository = importRepository;
        _rowValidator = rowValidator;
    }

    public DatasetType DatasetType =>
        DatasetType.RolePermissions;

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
                        DatasetType.RolePermissions);

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
                    row.Get(ImportField.ApplicationName);

                var roleName =
                    row.Get(ImportField.RoleName);

                var permissionName =
                    row.Get(ImportField.PermissionName);

                if (string.IsNullOrWhiteSpace(applicationName))
                {
                    errors.Add(
                        CreateError(
                            rowNumber,
                            "ApplicationName is required."));

                    continue;
                }

                if (string.IsNullOrWhiteSpace(roleName))
                {
                    errors.Add(
                        CreateError(
                            rowNumber,
                            "RoleName is required."));

                    continue;
                }

                if (string.IsNullOrWhiteSpace(permissionName))
                {
                    errors.Add(
                        CreateError(
                            rowNumber,
                            "PermissionName is required."));

                    continue;
                }

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

                var permission =
                    await _importRepository
                        .GetPermissionByNameAsync(
                            permissionName,
                            cancellationToken);

                if (permission is null)
                {
                    errors.Add(
                        CreateError(
                            rowNumber,
                            $"Permission '{permissionName}' does not exist."));

                    continue;
                }

                var existingRolePermission =
                    await _importRepository
                        .GetRolePermissionAsync(
                            role.Id,
                            permission.Id,
                            cancellationToken);

                if (existingRolePermission is not null)
                {
                    successfulRecords++;
                    continue;
                }

                var rolePermission =
                    RolePermission.Create(
                        role.Id,
                        permission.Id);

                await _importRepository
                    .AddRolePermissionAsync(
                        rolePermission,
                        cancellationToken);

                successfulRecords++;
            }
            catch (Exception exception)
            {
                errors.Add(
                    CreateError(
                        rowNumber,
                        exception.Message));
            }
        }

        await _importRepository.SaveChangesAsync(
            cancellationToken);

        if (errors.Count == 0)
        {
            return ImportProcessingResult.Success();
        }

        return ImportProcessingResult.PartialFailure(
            errors);
    }

    private static ImportProcessingError CreateError(
        int rowNumber,
        string errorMessage)
    {
        return new ImportProcessingError
        {
            RowNumber = rowNumber,
            ErrorMessage = errorMessage
        };
    }
}