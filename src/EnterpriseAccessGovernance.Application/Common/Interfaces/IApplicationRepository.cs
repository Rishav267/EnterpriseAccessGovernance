using EnterpriseAccessGovernance.Application.Features.ApplicationRoles.DTOs;
using EnterpriseAccessGovernance.Application.Features.Applications.DTOs;

namespace EnterpriseAccessGovernance.Application.Common.Interfaces;

public interface IApplicationRepository
{
    Task<IReadOnlyCollection<ApplicationListItemDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<ApplicationDetailsDto?> GetByIdAsync(
    Guid applicationId,
    CancellationToken cancellationToken = default);
}