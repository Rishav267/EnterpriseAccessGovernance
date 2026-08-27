using EnterpriseAccessGovernance.Application.Common.Interfaces;
using EnterpriseAccessGovernance.Application.Common.Models;
using EnterpriseAccessGovernance.Domain.Entities;

namespace EnterpriseAccessGovernance.Infrastructure.Import.Processors;

public sealed class RoleImportProcessor
: IImportDatasetProcessor
{
    private readonly IImportRepository _importRepository;

public RoleImportProcessor(
    IImportRepository importRepository)
    {
        _importRepository =
            importRepository
            ?? throw new ArgumentNullException(
                nameof(importRepository));
    }

    public DatasetType DatasetType =>
        DatasetType.Roles;

    public async Task<ImportProcessingResult> ProcessAsync(
        IReadOnlyCollection<CanonicalImportRow> rows,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(rows);

        var errors =
            new List<ImportProcessingError>();

        var rowNumber = 1;

        foreach (var row in rows)
        {
            cancellationToken.ThrowIfCancellationRequested();

            rowNumber++;

            try
            {
                var applicationName =
                    row.Get(ImportField.ApplicationName);

                var roleName =
                    row.Get(ImportField.RoleName);

                if (string.IsNullOrWhiteSpace(applicationName))
                {
                    errors.Add(
                        new ImportProcessingError
                        {
                            RowNumber = rowNumber,
                            ErrorMessage =
                                "ApplicationName is required."
                        });

                    continue;
                }

                if (string.IsNullOrWhiteSpace(roleName))
                {
                    errors.Add(
                        new ImportProcessingError
                        {
                            RowNumber = rowNumber,
                            ErrorMessage =
                                "RoleName is required."
                        });

                    continue;
                }

                var application =
                    await _importRepository
                        .GetApplicationByNameAsync(
                            applicationName.Trim(),
                            cancellationToken);

                if (application is null)
                {
                    errors.Add(
                        new ImportProcessingError
                        {
                            RowNumber = rowNumber,
                            ErrorMessage =
                                $"Application '{applicationName}' " +
                                "does not exist."
                        });

                    continue;
                }

                var roleCode =
                    row.Get(ImportField.RoleId);

                if (string.IsNullOrWhiteSpace(roleCode))
                {
                    roleCode =
                        CreateRoleCode(roleName);
                }

                var existingRole =
                    await _importRepository
                        .GetRoleByCodeAsync(
                            application.Id,
                            roleCode,
                            cancellationToken);

                if (existingRole is not null)
                {
                    // Role already exists.
                    // Treat the import as idempotent.
                    continue;
                }

                var role =
                    ApplicationRole.Create(
                        application.Id,
                        roleName.Trim(),
                        roleCode.Trim());

                await _importRepository
                    .AddRoleAsync(
                        role,
                        cancellationToken);
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

        if (errors.Count > 0)
        {
            return ImportProcessingResult.PartialFailure(
                errors);
        }

        return ImportProcessingResult.Success();
    }

    private static string CreateRoleCode(
        string roleName)
    {
        var words =
            roleName
                .Split(
                    ' ',
                    StringSplitOptions.RemoveEmptyEntries |
                    StringSplitOptions.TrimEntries);

        var code =
            string.Concat(
                words.Select(
                    word =>
                        char.ToUpperInvariant(word[0])));

        if (string.IsNullOrWhiteSpace(code))
        {
            throw new InvalidOperationException(
                "Unable to generate role code.");
        }

        return code;
    }
}
