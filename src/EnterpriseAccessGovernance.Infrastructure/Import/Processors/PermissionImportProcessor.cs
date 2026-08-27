using EnterpriseAccessGovernance.Application.Common.Interfaces;
using EnterpriseAccessGovernance.Application.Common.Models;
using EnterpriseAccessGovernance.Domain.Entities;

namespace EnterpriseAccessGovernance.Infrastructure.Import.Processors;

public sealed class PermissionImportProcessor
    : IImportDatasetProcessor
{
    private readonly IImportRepository _importRepository;
    private readonly IImportRowValidator _rowValidator;

    public PermissionImportProcessor(
        IImportRepository importRepository,
        IImportRowValidator rowValidator)
    {
        _importRepository = importRepository;
        _rowValidator = rowValidator;
    }

    public DatasetType DatasetType =>
        DatasetType.Permissions;

    public async Task<ImportProcessingResult> ProcessAsync(
        IReadOnlyCollection<CanonicalImportRow> rows,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(rows);

        var errors =
            new List<ImportProcessingError>();

        foreach (var row in rows)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var validationResult =
                    _rowValidator.Validate(
                        row,
                        DatasetType.Permissions);

                if (!validationResult.IsValid)
                {
                    errors.Add(
                        new ImportProcessingError
                        {
                            RowNumber = 1,
                            ErrorMessage = string.Join(
                                " ",
                                validationResult.Errors)
                        });

                    continue;
                }

                var permissionName =
                    row.Get(ImportField.PermissionName)!;

                var permissionCode =
                    row.Get(ImportField.PermissionId);

                if (string.IsNullOrWhiteSpace(permissionCode))
                {
                    permissionCode =
                        CreatePermissionCode(permissionName);
                }

                var existingPermission =
                    await _importRepository
                        .GetPermissionByCodeAsync(
                            permissionCode,
                            cancellationToken);

                if (existingPermission is not null)
                {
                    continue;
                }

                var existingPermissionByName =
                    await _importRepository
                        .GetPermissionByNameAsync(
                            permissionName,
                            cancellationToken);

                if (existingPermissionByName is not null)
                {
                    continue;
                }

                var permission =
                    Permission.Create(
                        permissionName,
                        permissionCode);

                await _importRepository
                    .AddPermissionAsync(
                        permission,
                        cancellationToken);
            }
            catch (Exception exception)
            {
                errors.Add(
                    new ImportProcessingError
                    {
                        RowNumber = 1,
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

        return ImportProcessingResult.PartialFailure(
            errors);
    }

    private static string CreatePermissionCode(
        string permissionName)
    {
        var words =
            permissionName
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
                "Unable to generate permission code.");
        }

        return code;
    }
}