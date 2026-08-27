using EnterpriseAccessGovernance.Application.Features.Applications.DTOs;

namespace EnterpriseAccessGovernance.Application.Features.Applications;

public interface IApplicationService
{
    Task<IReadOnlyCollection<ApplicationListItemDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<ApplicationDetailsDto?> GetByIdAsync(
    Guid applicationId,
    CancellationToken cancellationToken = default);
}