using EnterpriseAccessGovernance.Application.Common.Interfaces;
using EnterpriseAccessGovernance.Domain.Entities;
using EnterpriseAccessGovernance.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseAccessGovernance.Infrastructure.Repositories;

public sealed class ImportDataRepository
    : IImportDataRepository
{
    private readonly EnterpriseAccessGovernanceDbContext _dbContext;

    public ImportDataRepository(
        EnterpriseAccessGovernanceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Department?> FindDepartmentAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Departments
            .FirstOrDefaultAsync(
                x => x.Code == code,
                cancellationToken);
    }

    public async Task AddDepartmentAsync(
        Department department,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(department);

        await _dbContext.Departments.AddAsync(
            department,
            cancellationToken);
    }

    public Task<Employee?> FindEmployeeAsync(
        string employeeNumber,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Employees
            .FirstOrDefaultAsync(
                x => x.EmployeeNumber == employeeNumber,
                cancellationToken);
    }

    public async Task AddEmployeeAsync(
        Employee employee,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(employee);

        await _dbContext.Employees.AddAsync(
            employee,
            cancellationToken);
    }

    public Task<EnterpriseApplication?> FindApplicationAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Applications
            .FirstOrDefaultAsync(
                x => x.Code == code,
                cancellationToken);
    }

    public async Task AddApplicationAsync(
        EnterpriseApplication application,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(application);

        await _dbContext.Applications.AddAsync(
            application,
            cancellationToken);
    }

    public Task<ApplicationRole?> FindRoleAsync(
        Guid applicationId,
        string code,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.ApplicationRoles
            .FirstOrDefaultAsync(
                x =>
                    x.EnterpriseApplicationId == applicationId &&
                    x.Code == code,
                cancellationToken);
    }

    public async Task AddRoleAsync(
        ApplicationRole role,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(role);

        await _dbContext.ApplicationRoles.AddAsync(
            role,
            cancellationToken);
    }

    public Task<Permission?> FindPermissionAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Permissions
            .FirstOrDefaultAsync(
                x => x.Code == code,
                cancellationToken);
    }

    public async Task AddPermissionAsync(
        Permission permission,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(permission);

        await _dbContext.Permissions.AddAsync(
            permission,
            cancellationToken);
    }

    public Task<bool> RolePermissionExistsAsync(
        Guid applicationRoleId,
        Guid permissionId,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.RolePermissions
            .AnyAsync(
                x =>
                    x.ApplicationRoleId == applicationRoleId &&
                    x.PermissionId == permissionId,
                cancellationToken);
    }

    public async Task AddRolePermissionAsync(
        RolePermission rolePermission,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(rolePermission);

        await _dbContext.RolePermissions.AddAsync(
            rolePermission,
            cancellationToken);
    }

    public Task<AccessAssignment?> FindAccessAssignmentAsync(
        Guid employeeId,
        Guid applicationId,
        Guid roleId,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.AccessAssignments
            .FirstOrDefaultAsync(
                x =>
                    x.EmployeeId == employeeId &&
                    x.EnterpriseApplicationId == applicationId &&
                    x.ApplicationRoleId == roleId,
                cancellationToken);
    }

    public async Task AddAccessAssignmentAsync(
        AccessAssignment accessAssignment,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(accessAssignment);

        await _dbContext.AccessAssignments.AddAsync(
            accessAssignment,
            cancellationToken);
    }

    public async Task AddLoginActivityAsync(
        LoginActivity loginActivity,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(loginActivity);

        await _dbContext.LoginActivities.AddAsync(
            loginActivity,
            cancellationToken);
    }

    public Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(
            cancellationToken);
    }
}