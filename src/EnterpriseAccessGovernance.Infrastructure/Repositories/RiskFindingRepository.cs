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

    public async Task<bool> ExistsOpenFindingAsync(
    Guid employeeId,
    string ruleCode,
    CancellationToken cancellationToken = default)
    {
        return await _dbContext.RiskFindings
            .AsNoTracking()
            .AnyAsync(
                x =>
                    x.EmployeeId == employeeId &&
                    x.RuleCode == ruleCode &&
                    x.Status == Domain.Enums.RiskStatus.Open,
                cancellationToken);
    }

    public async Task AddAsync(
        RiskFinding riskFinding,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.RiskFindings.AddAsync(
            riskFinding,
            cancellationToken);
    }

    public async Task<
    (IReadOnlyCollection<RiskFindingListItemDto> Items, int TotalCount)>
    GetPagedAsync(
        RiskFindingQueryDto query,
        CancellationToken cancellationToken = default)
    {
        var findings =
            _dbContext.RiskFindings
                .AsNoTracking()
                .AsQueryable();

        // -------------------------------------------------
        // Filters
        // -------------------------------------------------

        if (!string.IsNullOrWhiteSpace(query.Severity) &&
            Enum.TryParse<Domain.Enums.RiskSeverity>(
                query.Severity,
                true,
                out var severity))
        {
            findings = findings.Where(x =>
                x.Severity == severity);
        }

        if (!string.IsNullOrWhiteSpace(query.Status) &&
            Enum.TryParse<Domain.Enums.RiskStatus>(
                query.Status,
                true,
                out var status))
        {
            findings = findings.Where(x =>
                x.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(query.RuleCode))
        {
            var ruleCode =
                query.RuleCode
                    .Trim()
                    .ToUpperInvariant();

            findings = findings.Where(x =>
                x.RuleCode == ruleCode);
        }

        if (query.EmployeeId.HasValue)
        {
            findings = findings.Where(x =>
                x.EmployeeId == query.EmployeeId.Value);
        }

        // -------------------------------------------------
        // Count
        // -------------------------------------------------

        var totalCount =
            await findings.CountAsync(
                cancellationToken);

        // -------------------------------------------------
        // Sorting + Pagination + Projection
        // -------------------------------------------------

        var items =
            await findings
                .OrderByDescending(x => x.Severity)
                .ThenByDescending(x => x.DetectedAtUtc)
                .Skip(
                    (query.PageNumber - 1) *
                    query.PageSize)
                .Take(query.PageSize)
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

                    Severity = x.Severity.ToString(),

                    Status = x.Status.ToString(),

                    DetectedAtUtc = x.DetectedAtUtc,

                    ResolvedAtUtc = x.ResolvedAtUtc
                })
                .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<RiskFindingSummaryDto>
    GetSummaryAsync(
        CancellationToken cancellationToken = default)
    {
        var summary =
            await _dbContext.RiskFindings
                .AsNoTracking()
                .GroupBy(_ => 1)
                .Select(group => new RiskFindingSummaryDto
                {
                    TotalOpen =
                        group.Count(x =>
                            x.Status ==
                            Domain.Enums.RiskStatus.Open),

                    Critical =
                        group.Count(x =>
                            x.Status ==
                                Domain.Enums.RiskStatus.Open &&
                            x.Severity ==
                                Domain.Enums.RiskSeverity.Critical),

                    High =
                        group.Count(x =>
                            x.Status ==
                                Domain.Enums.RiskStatus.Open &&
                            x.Severity ==
                                Domain.Enums.RiskSeverity.High),

                    Medium =
                        group.Count(x =>
                            x.Status ==
                                Domain.Enums.RiskStatus.Open &&
                            x.Severity ==
                                Domain.Enums.RiskSeverity.Medium),

                    Low =
                        group.Count(x =>
                            x.Status ==
                                Domain.Enums.RiskStatus.Open &&
                            x.Severity ==
                                Domain.Enums.RiskSeverity.Low),

                    TotalResolved =
                        group.Count(x =>
                            x.Status ==
                            Domain.Enums.RiskStatus.Resolved),

                    TotalIgnored =
                        group.Count(x =>
                            x.Status ==
                            Domain.Enums.RiskStatus.Ignored)
                })
                .FirstOrDefaultAsync(cancellationToken);

        return summary ?? new RiskFindingSummaryDto();
    }
}