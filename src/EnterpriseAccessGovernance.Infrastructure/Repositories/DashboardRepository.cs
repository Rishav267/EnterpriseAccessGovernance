using EnterpriseAccessGovernance.Application.Common.Interfaces;
using EnterpriseAccessGovernance.Application.Features.Dashboard.DTOs;
using EnterpriseAccessGovernance.Domain.Enums;
using EnterpriseAccessGovernance.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseAccessGovernance.Infrastructure.Repositories;

public sealed class DashboardRepository : IDashboardRepository
{
    private readonly EnterpriseAccessGovernanceDbContext _dbContext;

    public DashboardRepository(
        EnterpriseAccessGovernanceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DashboardSummaryDto> GetSummaryAsync(
        CancellationToken cancellationToken = default)
    {
        var totalEmployees =
            await _dbContext.Employees
                .AsNoTracking()
                .CountAsync(cancellationToken);

        var activeEmployees =
            await _dbContext.Employees
                .AsNoTracking()
                .CountAsync(
                    x => x.EmploymentStatus == EmploymentStatus.Active,
                    cancellationToken);

        var inactiveEmployees =
            await _dbContext.Employees
                .AsNoTracking()
                .CountAsync(
                    x => x.EmploymentStatus != EmploymentStatus.Active,
                    cancellationToken);

        var totalApplications =
            await _dbContext.Applications
                .AsNoTracking()
                .CountAsync(cancellationToken);

        var totalAccessAssignments =
            await _dbContext.AccessAssignments
                .AsNoTracking()
                .CountAsync(cancellationToken);

        var activeAccessAssignments =
            await _dbContext.AccessAssignments
                .AsNoTracking()
                .CountAsync(
                    x => x.Status == AccessStatus.Active,
                    cancellationToken);

        var pendingReviews =
            await _dbContext.AccessAssignments
                .AsNoTracking()
                .CountAsync(
                    x => x.Status == AccessStatus.PendingReview,
                    cancellationToken);

        var highRiskUsers =
            await _dbContext.RiskFindings
                .AsNoTracking()
                .Where(x => x.Status == RiskStatus.Open)
                .Where(
                    x => x.Severity == RiskSeverity.High ||
                         x.Severity == RiskSeverity.Critical)
                .Select(x => x.EmployeeId)
                .Distinct()
                .CountAsync(cancellationToken);

        var openRiskFindings =
            await _dbContext.RiskFindings
                .AsNoTracking()
                .CountAsync(
                    x => x.Status == RiskStatus.Open,
                    cancellationToken);

        return new DashboardSummaryDto
        {
            TotalEmployees = totalEmployees,
            ActiveEmployees = activeEmployees,
            InactiveEmployees = inactiveEmployees,
            TotalApplications = totalApplications,
            TotalAccessAssignments = totalAccessAssignments,
            ActiveAccessAssignments = activeAccessAssignments,
            PendingReviews = pendingReviews,
            HighRiskUsers = highRiskUsers,
            OpenRiskFindings = openRiskFindings
        };
    }
}