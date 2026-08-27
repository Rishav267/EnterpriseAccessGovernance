using EnterpriseAccessGovernance.Application.Features.ApplicationRoles.DTOs;

namespace EnterpriseAccessGovernance.Application.Common.Interfaces;

public interface IApplicationRoleRepository
{
    Task<IReadOnlyCollection<ApplicationRoleListItemDto>>
        GetByApplicationIdAsync(
            Guid applicationId,
            CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<PermissionListItemDto>>
    GetPermissionsAsync(
        Guid applicationId,
        Guid roleId,
        CancellationToken cancellationToken = default);
}