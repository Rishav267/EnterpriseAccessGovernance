using EnterpriseAccessGovernance.Application.Common.Interfaces;
using EnterpriseAccessGovernance.Application.Features.AccessAssignments.DTOs;
using EnterpriseAccessGovernance.Domain.Entities;
using EnterpriseAccessGovernance.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseAccessGovernance.Infrastructure.Repositories;

public sealed class AccessAssignmentRepository
    : IAccessAssignmentRepository
{
    private readonly EnterpriseAccessGovernanceDbContext _dbContext;

    public AccessAssignmentRepository(
        EnterpriseAccessGovernanceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<AccessAssignmentListItemDto>>
        GetByEmployeeIdAsync(
            Guid employeeId,
            CancellationToken cancellationToken = default)
    {
        return await _dbContext.AccessAssignments
            .AsNoTracking()
            .Where(x => x.EmployeeId == employeeId)
            .Select(x => new AccessAssignmentListItemDto
            {
                Id = x.Id,

                EmployeeId = x.EmployeeId,

                EnterpriseApplicationId =
                    x.EnterpriseApplicationId,

                ApplicationName =
                    x.EnterpriseApplication != null
                        ? x.EnterpriseApplication.Name
                        : string.Empty,

                ApplicationCode =
                    x.EnterpriseApplication != null
                        ? x.EnterpriseApplication.Code
                        : string.Empty,

                ApplicationRoleId =
                    x.ApplicationRoleId,

                RoleName =
                    x.ApplicationRole != null
                        ? x.ApplicationRole.Name
                        : string.Empty,

                RoleCode =
                    x.ApplicationRole != null
                        ? x.ApplicationRole.Code
                        : string.Empty,

                IsHighPrivilege =
                    x.ApplicationRole != null &&
                    x.ApplicationRole.IsHighPrivilege,

                Status =
                    x.Status.ToString(),

                GrantedAtUtc =
                    x.GrantedAtUtc,

                ExpiresAtUtc =
                    x.ExpiresAtUtc,

                RevokedAtUtc =
                    x.RevokedAtUtc,

                LastReviewedAtUtc =
                    x.LastReviewedAtUtc
            })
            .OrderBy(x => x.ApplicationName)
            .ThenBy(x => x.RoleName)
            .ToListAsync(cancellationToken);
    }

    public async Task<AccessAssignment?> GetByIdAsync(
    Guid accessAssignmentId,
    CancellationToken cancellationToken = default)
    {
        return await _dbContext.AccessAssignments
            .SingleOrDefaultAsync(
                x => x.Id == accessAssignmentId,
                cancellationToken);
    }

    public Task SaveChangesAsync(
    CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(
            cancellationToken);
    }
}