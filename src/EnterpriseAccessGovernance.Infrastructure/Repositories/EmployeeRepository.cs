using EnterpriseAccessGovernance.Application.Common.Interfaces;
using EnterpriseAccessGovernance.Application.Features.Employees.DTOs;
using EnterpriseAccessGovernance.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseAccessGovernance.Infrastructure.Repositories;

public sealed class EmployeeRepository : IEmployeeRepository
{
    private readonly EnterpriseAccessGovernanceDbContext _dbContext;

    public EmployeeRepository(
        EnterpriseAccessGovernanceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<EmployeeListItemDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var employees =
            await _dbContext.Employees
                .AsNoTracking()
                .Include(x => x.Department)
                .Include(x => x.Manager)
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .Select(x => new EmployeeListItemDto
                {
                    Id = x.Id,
                    EmployeeNumber = x.EmployeeNumber,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Email = x.Email,
                    EmploymentStatus =
                        x.EmploymentStatus.ToString(),
                    DepartmentId = x.DepartmentId,
                    DepartmentName =
                        x.Department != null
                            ? x.Department.Name
                            : string.Empty,
                    ManagerId = x.ManagerId,
                    ManagerName =
                        x.Manager != null
                            ? x.Manager.FirstName + " " +
                              x.Manager.LastName
                            : null
                })
                .ToListAsync(cancellationToken);

        return employees;
    }

    public async Task<EmployeeListItemDto?> GetByIdAsync(
    Guid id,
    CancellationToken cancellationToken = default)
    {
        return await _dbContext.Employees
            .AsNoTracking()
            .Include(x => x.Department)
            .Include(x => x.Manager)
            .Where(x => x.Id == id)
            .Select(x => new EmployeeListItemDto
            {
                Id = x.Id,
                EmployeeNumber = x.EmployeeNumber,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                EmploymentStatus =
                    x.EmploymentStatus.ToString(),
                DepartmentId = x.DepartmentId,
                DepartmentName =
                    x.Department != null
                        ? x.Department.Name
                        : string.Empty,
                ManagerId = x.ManagerId,
                ManagerName =
                    x.Manager != null
                        ? x.Manager.FirstName + " " +
                          x.Manager.LastName
                        : null
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}