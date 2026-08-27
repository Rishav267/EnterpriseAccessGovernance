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

    public async Task<IReadOnlyCollection<DashboardAccessDetailDto>>
        GetAccessDetailsAsync(
            string? employeeName,
            string? departmentName,
            string? applicationName,
            string? roleName,
            string? status,
            CancellationToken cancellationToken = default)
    {
        var query = _dbContext.AccessAssignments
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(employeeName))
        {
            var search = employeeName.Trim();

            query = query.Where(x =>
                (x.Employee != null &&
                 (
                     x.Employee.FirstName.Contains(search) ||
                     x.Employee.LastName.Contains(search) ||
                     x.Employee.Email.Contains(search)
                 )));
        }

        if (!string.IsNullOrWhiteSpace(departmentName))
        {
            var search = departmentName.Trim();

            query = query.Where(x =>
                x.Employee != null &&
                x.Employee.Department != null &&
                x.Employee.Department.Name.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(applicationName))
        {
            var search = applicationName.Trim();

            query = query.Where(x =>
                x.EnterpriseApplication != null &&
                x.EnterpriseApplication.Name.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(roleName))
        {
            var search = roleName.Trim();

            query = query.Where(x =>
                x.ApplicationRole != null &&
                x.ApplicationRole.Name.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(status) &&
            Enum.TryParse<AccessStatus>(
                status,
                true,
                out var parsedStatus))
        {
            query = query.Where(
                x => x.Status == parsedStatus);
        }

        return await query
            .Select(x => new DashboardAccessDetailDto
            {
                EmployeeId = x.EmployeeId,

                EmployeeName =
                    x.Employee != null
                        ? x.Employee.FirstName + " " +
                          x.Employee.LastName
                        : string.Empty,

                DepartmentName =
                    x.Employee != null &&
                    x.Employee.Department != null
                        ? x.Employee.Department.Name
                        : string.Empty,

                ApplicationName =
                    x.EnterpriseApplication != null
                        ? x.EnterpriseApplication.Name
                        : string.Empty,

                RoleName =
                    x.ApplicationRole != null
                        ? x.ApplicationRole.Name
                        : string.Empty,

                Status =
                    x.Status.ToString(),

                IsHighPrivilege =
                    x.ApplicationRole != null &&
                    x.ApplicationRole.IsHighPrivilege,

                ExpiresAtUtc =
                    x.ExpiresAtUtc
            })
            .OrderBy(x => x.EmployeeName)
            .ThenBy(x => x.ApplicationName)
            .ThenBy(x => x.RoleName)
            .ToListAsync(cancellationToken);
    }

    public async Task<DashboardAccessSummaryDto>
        GetAccessSummaryAsync(
            CancellationToken cancellationToken = default)
    {
        var accessByStatus =
            await _dbContext.AccessAssignments
                .AsNoTracking()
                .GroupBy(x => x.Status)
                .Select(g => new DashboardAccessStatusDto
                {
                    Status = g.Key.ToString(),
                    Count = g.Count()
                })
                .OrderBy(x => x.Status)
                .ToListAsync(cancellationToken);

        var accessByApplication =
            await _dbContext.AccessAssignments
                .AsNoTracking()
                .GroupBy(x => x.EnterpriseApplicationId)
                .Select(g => new DashboardApplicationAccessDto
                {
                    ApplicationName =
                        g.Select(x =>
                            x.EnterpriseApplication != null
                                ? x.EnterpriseApplication.Name
                                : string.Empty)
                         .FirstOrDefault() ?? string.Empty,

                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .ToListAsync(cancellationToken);

        var highRiskUsers =
            await _dbContext.RiskFindings
                .AsNoTracking()
                .Where(x =>
                    x.Status == RiskStatus.Open &&
                    (
                        x.Severity == RiskSeverity.High ||
                        x.Severity == RiskSeverity.Critical
                    ))
                .Select(x => x.EmployeeId)
                .Distinct()
                .CountAsync(cancellationToken);

        var normalUsers =
            await _dbContext.Employees
                .AsNoTracking()
                .CountAsync(
                    x => !x.RiskFindings.Any(
                        r =>
                            r.Status == RiskStatus.Open &&
                            (
                                r.Severity == RiskSeverity.High ||
                                r.Severity == RiskSeverity.Critical
                            )),
                    cancellationToken);

        return new DashboardAccessSummaryDto
        {
            AccessByStatus = accessByStatus,

            AccessByApplication = accessByApplication,

            HighRiskUsers = highRiskUsers,

            NormalUsers = normalUsers
        };
    }
}