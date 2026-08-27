using EnterpriseAccessGovernance.Application.Common.Interfaces;
using EnterpriseAccessGovernance.Application.Features.ApplicationRoles.DTOs;
using EnterpriseAccessGovernance.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseAccessGovernance.Infrastructure.Repositories;

public sealed class ApplicationRoleRepository
    : IApplicationRoleRepository
{
    private readonly EnterpriseAccessGovernanceDbContext _dbContext;

    public ApplicationRoleRepository(
        EnterpriseAccessGovernanceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<
        IReadOnlyCollection<ApplicationRoleListItemDto>>
        GetByApplicationIdAsync(
            Guid applicationId,
            CancellationToken cancellationToken = default)
    {
        return await _dbContext.ApplicationRoles
            .AsNoTracking()
            .Where(x =>
                x.EnterpriseApplicationId == applicationId)
            .OrderBy(x => x.Name)
            .Select(x => new ApplicationRoleListItemDto
            {
                Id = x.Id,

                EnterpriseApplicationId =
                    x.EnterpriseApplicationId,

                Name = x.Name,

                Code = x.Code,

                IsHighPrivilege =
                    x.IsHighPrivilege,

                PermissionCount =
                    x.RolePermissions.Count
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<PermissionListItemDto>>
    GetPermissionsAsync(
        Guid applicationId,
        Guid roleId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.RolePermissions
            .AsNoTracking()
            .Where(x =>
                x.ApplicationRoleId == roleId &&
                x.ApplicationRole!.EnterpriseApplicationId == applicationId)
            .Select(x => new PermissionListItemDto
            {
                Id = x.PermissionId,
                Name = x.Permission!.Name,
                Code = x.Permission.Code,
                Description = x.Permission.Description
            })
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }
}