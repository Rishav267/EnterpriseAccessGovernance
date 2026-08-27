using EnterpriseAccessGovernance.Domain.Entities;

namespace EnterpriseAccessGovernance.Application.Common.Interfaces;

public interface IImportDataRepository
{
    Task<Department?> FindDepartmentAsync(
        string code,
        CancellationToken cancellationToken = default);

    Task AddDepartmentAsync(
        Department department,
        CancellationToken cancellationToken = default);

    Task<Employee?> FindEmployeeAsync(
        string employeeNumber,
        CancellationToken cancellationToken = default);

    Task AddEmployeeAsync(
        Employee employee,
        CancellationToken cancellationToken = default);

    Task<EnterpriseApplication?> FindApplicationAsync(
        string code,
        CancellationToken cancellationToken = default);

    Task AddApplicationAsync(
        EnterpriseApplication application,
        CancellationToken cancellationToken = default);

    Task<ApplicationRole?> FindRoleAsync(
        Guid applicationId,
        string code,
        CancellationToken cancellationToken = default);

    Task AddRoleAsync(
        ApplicationRole role,
        CancellationToken cancellationToken = default);

    Task<Permission?> FindPermissionAsync(
        string code,
        CancellationToken cancellationToken = default);

    Task AddPermissionAsync(
        Permission permission,
        CancellationToken cancellationToken = default);

    Task<bool> RolePermissionExistsAsync(
        Guid applicationRoleId,
        Guid permissionId,
        CancellationToken cancellationToken = default);

    Task AddRolePermissionAsync(
        RolePermission rolePermission,
        CancellationToken cancellationToken = default);

    Task<AccessAssignment?> FindAccessAssignmentAsync(
        Guid employeeId,
        Guid applicationId,
        Guid roleId,
        CancellationToken cancellationToken = default);

    Task AddAccessAssignmentAsync(
        AccessAssignment accessAssignment,
        CancellationToken cancellationToken = default);

    Task AddLoginActivityAsync(
        LoginActivity loginActivity,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}