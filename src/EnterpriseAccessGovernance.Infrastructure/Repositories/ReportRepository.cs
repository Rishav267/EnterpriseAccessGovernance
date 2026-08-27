using EnterpriseAccessGovernance.Application.Common.Interfaces;
using EnterpriseAccessGovernance.Application.Features.Reports.DTOs;
using EnterpriseAccessGovernance.Domain.Enums;
using EnterpriseAccessGovernance.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseAccessGovernance.Infrastructure.Repositories;

public sealed class ReportRepository : IReportRepository
{
    private readonly EnterpriseAccessGovernanceDbContext _dbContext;

    public ReportRepository(
        EnterpriseAccessGovernanceDbContext dbContext)
    {
        _dbContext = dbContext
            ?? throw new ArgumentNullException(nameof(dbContext));
    }

    // =========================================================
    // High Risk Users
    // =========================================================

    public async Task<IReadOnlyCollection<HighRiskUserDto>>
    GetHighRiskUsersAsync(
        CancellationToken cancellationToken = default)
    {
        var findings = await _dbContext.RiskFindings
            .AsNoTracking()
            .Where(x =>
                x.Status == RiskStatus.Open &&
                (x.Severity == RiskSeverity.High ||
                 x.Severity == RiskSeverity.Critical))
            .Select(x => new
            {
                x.EmployeeId,

                EmployeeNumber =
                    x.Employee != null
                        ? x.Employee.EmployeeNumber
                        : string.Empty,

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

                x.Severity
            })
            .ToListAsync(cancellationToken);

        return findings
            .GroupBy(x => new
            {
                x.EmployeeId,
                x.EmployeeNumber,
                x.EmployeeName,
                x.DepartmentName
            })
            .Select(group =>
            {
                var highestSeverity =
                    group.Max(x => x.Severity);

                return new HighRiskUserDto
                {
                    EmployeeId =
                        group.Key.EmployeeId,

                    EmployeeNumber =
                        group.Key.EmployeeNumber,

                    EmployeeName =
                        group.Key.EmployeeName,

                    DepartmentName =
                        group.Key.DepartmentName,

                    RiskFindingCount =
                        group.Count(),

                    HighestSeverity =
                        highestSeverity.ToString()
                };
            })
            .OrderByDescending(x =>
                x.HighestSeverity == nameof(RiskSeverity.Critical) ? 4 :
                x.HighestSeverity == nameof(RiskSeverity.High) ? 3 :
                x.HighestSeverity == nameof(RiskSeverity.Medium) ? 2 :
                1)
            .ThenByDescending(x => x.RiskFindingCount)
            .ToList();
    }

    // =========================================================
    // Dormant Accounts
    // =========================================================

    public async Task<IReadOnlyCollection<DormantAccountDto>>
        GetDormantAccountsAsync(
            int dormantDays,
            CancellationToken cancellationToken = default)
    {
        if (dormantDays <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(dormantDays),
                "Dormant days must be greater than zero.");
        }

        var cutoffDateUtc =
            DateTime.UtcNow.AddDays(-dormantDays);

        return await _dbContext.Employees
            .AsNoTracking()
            .Select(employee => new
            {
                Employee = employee,

                LastLoginAtUtc =
                    employee.LoginActivities
                        .Select(x => (DateTime?)x.LoginAtUtc)
                        .Max(),

                ActiveAccessCount =
                    employee.AccessAssignments
                        .Count(x =>
                            x.Status == AccessStatus.Active)
            })
            .Where(x =>
                x.LastLoginAtUtc == null ||
                x.LastLoginAtUtc < cutoffDateUtc)
            .Select(x => new DormantAccountDto
            {
                EmployeeId =
                    x.Employee.Id,

                EmployeeNumber =
                    x.Employee.EmployeeNumber,

                EmployeeName =
                    x.Employee.FirstName + " " +
                    x.Employee.LastName,

                DepartmentName =
                    x.Employee.Department != null
                        ? x.Employee.Department.Name
                        : string.Empty,

                LastLoginAtUtc =
                    x.LastLoginAtUtc,

                ActiveAccessCount =
                    x.ActiveAccessCount
            })
            .OrderBy(x => x.LastLoginAtUtc)
            .ToListAsync(cancellationToken);
    }

    // =========================================================
    // Access By Department
    // =========================================================

    public async Task<IReadOnlyCollection<AccessByDepartmentDto>>
        GetAccessByDepartmentAsync(
            CancellationToken cancellationToken = default)
    {
        return await _dbContext.Employees
            .AsNoTracking()
            .GroupBy(employee => new
            {
                employee.DepartmentId,

                DepartmentName =
                    employee.Department != null
                        ? employee.Department.Name
                        : string.Empty
            })
            .Select(group => new AccessByDepartmentDto
            {
                DepartmentId =
                    group.Key.DepartmentId,

                DepartmentName =
                    group.Key.DepartmentName,

                EmployeeCount =
                    group.Count(),

                AccessAssignmentCount =
                    group
                        .SelectMany(x => x.AccessAssignments)
                        .Count(),

                ActiveAccessAssignmentCount =
                    group
                        .SelectMany(x => x.AccessAssignments)
                        .Count(x =>
                            x.Status == AccessStatus.Active)
            })
            .OrderBy(x => x.DepartmentName)
            .ToListAsync(cancellationToken);
    }

    // =========================================================
    // Access By Application
    // =========================================================

    public async Task<IReadOnlyCollection<AccessByApplicationDto>>
        GetAccessByApplicationAsync(
            CancellationToken cancellationToken = default)
    {
        return await _dbContext.AccessAssignments
            .AsNoTracking()
            .GroupBy(x => new
            {
                x.EnterpriseApplicationId,

                ApplicationName =
                    x.EnterpriseApplication != null
                        ? x.EnterpriseApplication.Name
                        : string.Empty
            })
            .Select(group => new AccessByApplicationDto
            {
                ApplicationId =
                    group.Key.EnterpriseApplicationId,

                ApplicationName =
                    group.Key.ApplicationName,

                EmployeeCount =
                    group
                        .Select(x => x.EmployeeId)
                        .Distinct()
                        .Count(),

                AccessAssignmentCount =
                    group.Count(),

                ActiveAccessAssignmentCount =
                    group.Count(x =>
                        x.Status == AccessStatus.Active)
            })
            .OrderBy(x => x.ApplicationName)
            .ToListAsync(cancellationToken);
    }

    // =========================================================
    // Pending Certifications
    // =========================================================

    public async Task<IReadOnlyCollection<PendingCertificationDto>>
        GetPendingCertificationsAsync(
            CancellationToken cancellationToken = default)
    {
        return await _dbContext.AccessAssignments
            .AsNoTracking()
            .Where(x =>
                x.Status == AccessStatus.PendingReview)
            .Select(x => new PendingCertificationDto
            {
                AccessAssignmentId =
                    x.Id,

                EmployeeId =
                    x.EmployeeId,

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

                GrantedAtUtc =
                    x.GrantedAtUtc,

                LastReviewedAtUtc =
                    x.LastReviewedAtUtc
            })
            .OrderBy(x => x.GrantedAtUtc)
            .ToListAsync(cancellationToken);
    }

    // =========================================================
    // Certification Summary
    // =========================================================

    public async Task<CertificationSummaryDto>
        GetCertificationSummaryAsync(
            CancellationToken cancellationToken = default)
    {
        var totalAssignments =
            await _dbContext.AccessAssignments
                .AsNoTracking()
                .CountAsync(cancellationToken);

        var reviewedAssignments =
            await _dbContext.AccessAssignments
                .AsNoTracking()
                .CountAsync(
                    x => x.LastReviewedAtUtc.HasValue,
                    cancellationToken);

        var pendingAssignments =
            await _dbContext.AccessAssignments
                .AsNoTracking()
                .CountAsync(
                    x => x.Status == AccessStatus.PendingReview,
                    cancellationToken);

        var approvedReviews =
            await _dbContext.CertificationReviews
                .AsNoTracking()
                .CountAsync(
                    x => x.Decision == CertificationDecision.Approved,
                    cancellationToken);

        var revokedReviews =
            await _dbContext.CertificationReviews
                .AsNoTracking()
                .CountAsync(
                    x => x.Decision == CertificationDecision.Revoked,
                    cancellationToken);

        var modificationRequests =
            await _dbContext.CertificationReviews
                .AsNoTracking()
                .CountAsync(
                    x =>
                        x.Decision ==
                        CertificationDecision.ModificationRequested,
                    cancellationToken);

        var completionPercentage =
            totalAssignments == 0
                ? 0
                : Math.Round(
                    reviewedAssignments * 100m /
                    totalAssignments,
                    2);

        return new CertificationSummaryDto
        {
            TotalAssignments =
                totalAssignments,

            ReviewedAssignments =
                reviewedAssignments,

            PendingAssignments =
                pendingAssignments,

            ApprovedReviews =
                approvedReviews,

            RevokedReviews =
                revokedReviews,

            ModificationRequests =
                modificationRequests,

            CompletionPercentage =
                completionPercentage
        };
    }
}