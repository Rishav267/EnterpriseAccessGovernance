using EnterpriseAccessGovernance.Domain.Entities;

namespace EnterpriseAccessGovernance.Application.Common.Interfaces;

public interface IImportRepository
{
    Task AddBatchAsync(
        ImportBatch importBatch,
        CancellationToken cancellationToken = default);

    Task AddErrorAsync(
        ImportError importError,
        CancellationToken cancellationToken = default);

    Task<Department?> GetDepartmentByNameAsync(
        string departmentName,
        CancellationToken cancellationToken = default);

    Task<Department?> GetDepartmentByCodeAsync(
        string departmentCode,
        CancellationToken cancellationToken = default);

    Task<Employee?> GetEmployeeByEmployeeNumberAsync(
        string employeeNumber,
        CancellationToken cancellationToken = default);

    Task<Employee?> GetEmployeeByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task AddDepartmentAsync(
        Department department,
        CancellationToken cancellationToken = default);

    Task AddEmployeeAsync(
        Employee employee,
        CancellationToken cancellationToken = default);

    Task<EnterpriseApplication?> GetApplicationByNameAsync(
        string applicationName,
        CancellationToken cancellationToken = default);

    Task<EnterpriseApplication?> GetApplicationByCodeAsync(
        string applicationCode,
        CancellationToken cancellationToken = default);

    Task AddApplicationAsync(
        EnterpriseApplication application,
        CancellationToken cancellationToken = default);

    Task<ApplicationRole?> GetRoleByCodeAsync(
        Guid applicationId,
        string roleCode,
        CancellationToken cancellationToken = default);

    Task AddRoleAsync(
        ApplicationRole role,
        CancellationToken cancellationToken = default);

    Task<Permission?> GetPermissionByNameAsync(
        string permissionName,
        CancellationToken cancellationToken = default);

    Task<Permission?> GetPermissionByCodeAsync(
        string permissionCode,
        CancellationToken cancellationToken = default);

    Task AddPermissionAsync(
        Permission permission,
        CancellationToken cancellationToken = default);

    Task<ApplicationRole?> GetRoleByNameAsync(
        Guid applicationId,
        string roleName,
        CancellationToken cancellationToken = default);

    Task<RolePermission?> GetRolePermissionAsync(
        Guid applicationRoleId,
        Guid permissionId,
        CancellationToken cancellationToken = default);

    Task AddRolePermissionAsync(
        RolePermission rolePermission,
        CancellationToken cancellationToken = default);

    Task<RiskFinding?> GetRiskFindingAsync(
        Guid employeeId,
        string ruleCode,
        CancellationToken cancellationToken = default);

    Task AddRiskFindingAsync(
        RiskFinding riskFinding,
        CancellationToken cancellationToken = default);

    Task<AccessAssignment?> GetAccessAssignmentAsync(
        Guid employeeId,
        Guid applicationId,
        Guid applicationRoleId,
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