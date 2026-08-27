using EnterpriseAccessGovernance.Application.Common.Interfaces;
using EnterpriseAccessGovernance.Application.Common.Models;
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

    public async Task<PagedResultDto<EmployeeListItemDto>> SearchAsync(
    EmployeeSearchRequest request,
    CancellationToken cancellationToken = default)
    {
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var query = _dbContext.Employees
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();

            query = query.Where(x =>
                x.EmployeeNumber.Contains(search) ||
                x.FirstName.Contains(search) ||
                x.LastName.Contains(search) ||
                x.Email.Contains(search));
        }

        if (request.DepartmentId.HasValue)
        {
            query = query.Where(x =>
                x.DepartmentId == request.DepartmentId);
        }

        if (request.ManagerId.HasValue)
        {
            query = query.Where(x =>
                x.ManagerId == request.ManagerId);
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            var status = request.Status.Trim();

            query = query.Where(x =>
                x.EmploymentStatus.ToString() == status);
        }

        if (!string.IsNullOrWhiteSpace(request.Application))
        {
            var application = request.Application.Trim();

            query = query.Where(x =>
                x.AccessAssignments.Any(a =>
                    a.EnterpriseApplication.Name.Contains(application)));
        }

        if (!string.IsNullOrWhiteSpace(request.Role))
        {
            var role = request.Role.Trim();

            query = query.Where(x =>
                x.AccessAssignments.Any(a =>
                    a.ApplicationRole.Name.Contains(role)));
        }

        var totalCount =
            await query.CountAsync(cancellationToken);

        var items =
            await query
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
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

        return new PagedResultDto<EmployeeListItemDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}