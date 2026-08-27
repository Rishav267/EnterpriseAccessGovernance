using EnterpriseAccessGovernance.Application.Features.Imports.DTOs;

namespace EnterpriseAccessGovernance.Application.Features.Imports.Interfaces;

public interface IImportService
{
    Task<ImportResponseDto> ImportAsync(
        Stream fileStream,
        string fileName,
        CancellationToken cancellationToken = default);
}