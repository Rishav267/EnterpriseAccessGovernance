using EnterpriseAccessGovernance.Application.Common.Interfaces;
using EnterpriseAccessGovernance.Application.Features.ApplicationRoles.DTOs;

namespace EnterpriseAccessGovernance.Application.Features.ApplicationRoles;

public sealed class ApplicationRoleService
    : IApplicationRoleService
{
    private readonly IApplicationRoleRepository
        _applicationRoleRepository;

    public ApplicationRoleService(
        IApplicationRoleRepository applicationRoleRepository)
    {
        _applicationRoleRepository =
            applicationRoleRepository
            ?? throw new ArgumentNullException(
                nameof(applicationRoleRepository));
    }

    public Task<IReadOnlyCollection<ApplicationRoleListItemDto>>
        GetByApplicationIdAsync(
            Guid applicationId,
            CancellationToken cancellationToken = default)
    {
        if (applicationId == Guid.Empty)
        {
            throw new ArgumentException(
                "Application ID is required.",
                nameof(applicationId));
        }

        return _applicationRoleRepository
            .GetByApplicationIdAsync(
                applicationId,
                cancellationToken);
    }

    public Task<IReadOnlyCollection<PermissionListItemDto>>
    GetPermissionsAsync(
        Guid applicationId,
        Guid roleId,
        CancellationToken cancellationToken = default)
    {
        if (applicationId == Guid.Empty)
        {
            throw new ArgumentException(
                "Application ID is required.",
                nameof(applicationId));
        }

        if (roleId == Guid.Empty)
        {
            throw new ArgumentException(
                "Role ID is required.",
                nameof(roleId));
        }

        return _applicationRoleRepository
            .GetPermissionsAsync(
                applicationId,
                roleId,
                cancellationToken);
    }
}