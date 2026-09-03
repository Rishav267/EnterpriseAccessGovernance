using EnterpriseAccessGovernance.Application.Common.Interfaces;
using EnterpriseAccessGovernance.Domain.Entities;
using EnterpriseAccessGovernance.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseAccessGovernance.Infrastructure.Repositories;

public sealed class ImportRepository : IImportRepository
{
    private readonly EnterpriseAccessGovernanceDbContext _dbContext;

    public ImportRepository(
        EnterpriseAccessGovernanceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddBatchAsync(
        ImportBatch importBatch,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(importBatch);

        await _dbContext.ImportBatches.AddAsync(
            importBatch,
            cancellationToken);
    }

    public async Task AddErrorAsync(
        ImportError importError,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(importError);

        await _dbContext.ImportErrors.AddAsync(
            importError,
            cancellationToken);
    }

    public Task<Department?> GetDepartmentByNameAsync(
        string departmentName,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Departments
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Name == departmentName,
                cancellationToken);
    }

    public Task<Department?> GetDepartmentByCodeAsync(
        string departmentCode,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Departments
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Code == departmentCode,
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

    public Task<Employee?> GetEmployeeByEmployeeNumberAsync(
        string employeeNumber,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.EmployeeNumber == employeeNumber,
                cancellationToken);
    }

    public Task<Employee?> GetEmployeeByEmailAsync(
    string email,
    CancellationToken cancellationToken = default)
    {
        return _dbContext.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Email == email,
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

    public Task<EnterpriseApplication?> GetApplicationByNameAsync(
        string applicationName,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Applications
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Name == applicationName,
                cancellationToken);
    }

    public Task<EnterpriseApplication?> GetApplicationByCodeAsync(
        string applicationCode,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Applications
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Code == applicationCode,
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

    public Task<ApplicationRole?> GetRoleByCodeAsync(
        Guid applicationId,
        string roleCode,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.ApplicationRoles
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x =>
                    x.EnterpriseApplicationId == applicationId &&
                    x.Code == roleCode,
                cancellationToken);
    }

    public Task<ApplicationRole?> GetRoleByNameAsync(
                Guid applicationId,
                string roleName,
                CancellationToken cancellationToken = default)
    {
        return _dbContext.ApplicationRoles
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x =>
                    x.EnterpriseApplicationId == applicationId &&
                    x.Name == roleName,
                cancellationToken);
    }

    public Task<RolePermission?> GetRolePermissionAsync(
    Guid applicationRoleId,
    Guid permissionId,
    CancellationToken cancellationToken = default)
    {
        return _dbContext.RolePermissions
            .AsNoTracking()
            .FirstOrDefaultAsync(
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

    public async Task AddRoleAsync(
    ApplicationRole role,
    CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(role);

        await _dbContext.ApplicationRoles.AddAsync(
            role,
            cancellationToken);

        var pendingRoles = _dbContext.ChangeTracker
            .Entries<ApplicationRole>()
            .Select(x => new
            {
                x.Entity.Id,
                x.Entity.EnterpriseApplicationId,
                x.Entity.Name,
                x.Entity.Code,
                x.State
            })
            .ToList();

        Console.WriteLine(
            $"Pending ApplicationRoles: {pendingRoles.Count}");

        foreach (var pendingRole in pendingRoles)
        {
            Console.WriteLine(
                $"Role: {pendingRole.Name}, " +
                $"Code: {pendingRole.Code}, " +
                $"AppId: {pendingRole.EnterpriseApplicationId}, " +
                $"State: {pendingRole.State}");
        }
    }

    public Task<Permission?> GetPermissionByNameAsync(
    string permissionName,
    CancellationToken cancellationToken = default)
    {
        return _dbContext.Permissions
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Name == permissionName,
                cancellationToken);
    }

    public Task<Permission?> GetPermissionByCodeAsync(
        string permissionCode,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Permissions
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Code == permissionCode,
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

    public Task<AccessAssignment?> GetAccessAssignmentAsync(
    Guid employeeId,
    Guid applicationId,
    Guid applicationRoleId,
    CancellationToken cancellationToken = default)
    {
        return _dbContext.AccessAssignments
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x =>
                    x.EmployeeId == employeeId &&
                    x.EnterpriseApplicationId == applicationId &&
                    x.ApplicationRoleId == applicationRoleId,
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

    public Task<RiskFinding?> GetRiskFindingAsync(
    Guid employeeId,
    string ruleCode,
    CancellationToken cancellationToken = default)
    {
        return _dbContext.RiskFindings
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x =>
                    x.EmployeeId == employeeId &&
                    x.RuleCode == ruleCode,
                cancellationToken);
    }

    public async Task AddRiskFindingAsync(
        RiskFinding riskFinding,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(riskFinding);

        await _dbContext.RiskFindings.AddAsync(
            riskFinding,
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

    public async Task SaveChangesAsync(
    CancellationToken cancellationToken = default)
    {
        var entriesBeforeSave = _dbContext.ChangeTracker
            .Entries()
            .Where(x => x.State != EntityState.Unchanged)
            .ToList();

        Console.WriteLine(
            $"EF pending entities before SaveChanges: " +
            $"{entriesBeforeSave.Count}");

        foreach (var entry in entriesBeforeSave)
        {
            Console.WriteLine(
                $"Entity: {entry.Entity.GetType().Name}, " +
                $"State: {entry.State}");
        }

        var result =
            await _dbContext.SaveChangesAsync(
                cancellationToken);

        Console.WriteLine(
            $"EF SaveChanges affected: {result} rows");

        var rolesInDatabase =
            await _dbContext.ApplicationRoles
                .AsNoTracking()
                .CountAsync(cancellationToken);

        Console.WriteLine(
            $"ApplicationRoles in DB after SaveChanges: " +
            $"{rolesInDatabase}");
    }
}