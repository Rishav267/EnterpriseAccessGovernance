using EnterpriseAccessGovernance.Application.Common.Interfaces;
using EnterpriseAccessGovernance.Application.Features.ApplicationRoles.DTOs;
using EnterpriseAccessGovernance.Application.Features.Applications.DTOs;
using EnterpriseAccessGovernance.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseAccessGovernance.Infrastructure.Repositories;

public sealed class ApplicationRepository : IApplicationRepository
{
    private readonly EnterpriseAccessGovernanceDbContext _dbContext;

    public ApplicationRepository(
        EnterpriseAccessGovernanceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<ApplicationListItemDto>>
        GetAllAsync(
            CancellationToken cancellationToken = default)
    {
        return await _dbContext.Applications
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new ApplicationListItemDto
            {
                Id = x.Id,
                Name = x.Name,
                Code = x.Code,
                Description = x.Description,
                RoleCount = x.Roles.Count
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<ApplicationDetailsDto?> GetByIdAsync(
    Guid applicationId,
    CancellationToken cancellationToken = default)
    {
        return await _dbContext.Applications
            .AsNoTracking()
            .Where(x => x.Id == applicationId)
            .Select(x => new ApplicationDetailsDto
            {
                Id = x.Id,
                Name = x.Name,
                Code = x.Code,
                Description = x.Description,
                RoleCount = x.Roles.Count
            })
            .SingleOrDefaultAsync(cancellationToken);
    }
}