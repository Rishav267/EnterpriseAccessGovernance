using EnterpriseAccessGovernance.Application.Common.Interfaces;
using EnterpriseAccessGovernance.Application.Features.RiskFindings.DTOs;
using EnterpriseAccessGovernance.Infrastructure.Persistence;
using EnterpriseAccessGovernance.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseAccessGovernance.Infrastructure.Repositories;

public sealed class RiskFindingRepository
    : IRiskFindingRepository
{
    private readonly EnterpriseAccessGovernanceDbContext _dbContext;

    public RiskFindingRepository(
        EnterpriseAccessGovernanceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<
        IReadOnlyCollection<RiskFindingListItemDto>>
        GetByEmployeeIdAsync(
            Guid employeeId,
            CancellationToken cancellationToken = default)
    {
        return await _dbContext.RiskFindings
            .AsNoTracking()
            .Include(x => x.Employee)
            .Where(x => x.EmployeeId == employeeId)
            .OrderByDescending(x => x.Severity)
            .ThenByDescending(x => x.DetectedAtUtc)
            .Select(x => new RiskFindingListItemDto
            {
                Id = x.Id,
                EmployeeId = x.EmployeeId,

                EmployeeName =
                    x.Employee != null
                        ? x.Employee.FirstName + " " +
                          x.Employee.LastName
                        : string.Empty,

                RuleCode = x.RuleCode,

                Description = x.Description,

                Severity =
                    x.Severity.ToString(),

                Status =
                    x.Status.ToString(),

                DetectedAtUtc =
                    x.DetectedAtUtc,

                ResolvedAtUtc =
                    x.ResolvedAtUtc
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<RiskFinding?> GetByIdAsync(
        Guid riskFindingId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.RiskFindings
            .SingleOrDefaultAsync(
                x => x.Id == riskFindingId,
                cancellationToken);
    }

    public Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(
            cancellationToken);
    }
}