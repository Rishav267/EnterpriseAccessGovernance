using EnterpriseAccessGovernance.Application.Common.Interfaces;
using EnterpriseAccessGovernance.Application.Features.RiskFindings.DTOs;
using EnterpriseAccessGovernance.Domain.Entities;
using EnterpriseAccessGovernance.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseAccessGovernance.Infrastructure.Repositories;

public sealed class RiskDetectionRepository
    : IRiskDetectionRepository
{
    private readonly EnterpriseAccessGovernanceDbContext _dbContext;

    public RiskDetectionRepository(
        EnterpriseAccessGovernanceDbContext dbContext)
    {
        _dbContext = dbContext
            ?? throw new ArgumentNullException(
                nameof(dbContext));
    }

    public async Task<
        IReadOnlyCollection<RiskDetectionEmployeeDto>>
        GetDetectionDataAsync(
            CancellationToken cancellationToken = default)
    {
        var employees =
            await _dbContext.Employees
                .AsNoTracking()
                .Select(employee => new RiskDetectionEmployeeDto
                {
                    EmployeeId = employee.Id,

                    EmployeeName =
                        employee.FirstName + " " +
                        employee.LastName,

                    EmploymentStatus =
                        employee.EmploymentStatus.ToString(),

                    LastLoginAtUtc =
                        employee.LoginActivities
                            .Select(x => (DateTime?)x.LoginAtUtc)
                            .Max(),

                    AccessAssignments =
                        employee.AccessAssignments
                            .Select(access => new RiskDetectionAccessDto
                            {
                                AccessAssignmentId =
                                    access.Id,

                                EnterpriseApplicationId =
                                    access.EnterpriseApplicationId,

                                ApplicationName =
                                    access.EnterpriseApplication != null
                                        ? access.EnterpriseApplication.Name
                                        : string.Empty,

                                RoleName =
                                    access.ApplicationRole != null
                                        ? access.ApplicationRole.Name
                                        : string.Empty,

                                IsHighPrivilege =
                                    access.ApplicationRole != null &&
                                    access.ApplicationRole.IsHighPrivilege,

                                Status =
                                    access.Status.ToString(),

                                ExpiresAtUtc =
                                    access.ExpiresAtUtc
                            })
                            .ToList()
                })
                .ToListAsync(cancellationToken);

        return employees;
    }

    public async Task<
        IReadOnlyCollection<RiskFinding>>
        GetOpenFindingsAsync(
            CancellationToken cancellationToken = default)
    {
        return await _dbContext.RiskFindings
            .Where(x =>
                x.Status == Domain.Enums.RiskStatus.Open)
            .ToListAsync(cancellationToken);
    }

    public async Task AddFindingAsync(
        RiskFinding finding,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.RiskFindings.AddAsync(
            finding,
            cancellationToken);
    }

    public Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(
            cancellationToken);
    }
}