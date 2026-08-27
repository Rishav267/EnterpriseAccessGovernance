using EnterpriseAccessGovernance.Application.Features.ApplicationRoles.DTOs;

namespace EnterpriseAccessGovernance.Application.Features.ApplicationRoles;

public interface IApplicationRoleService
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