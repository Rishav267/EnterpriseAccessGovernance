using EnterpriseAccessGovernance.Application.Common.Interfaces;
using EnterpriseAccessGovernance.Application.Common.Models;

namespace EnterpriseAccessGovernance.Infrastructure.Import.Detection;

public sealed class DatasetDetector : IDatasetDetector
{
    public DatasetDetectionResult Detect(
        IReadOnlyCollection<ImportField> mappedFields)
    {
        ArgumentNullException.ThrowIfNull(mappedFields);

        var fields = mappedFields.ToHashSet();

        var candidates = new List<DatasetType>();

        // ---------------------------------------------------------
        // Order matters.
        //
        // More specific datasets must be detected before generic
        // datasets.
        // ---------------------------------------------------------

        if (IsEmployeeDataset(fields))
        {
            candidates.Add(DatasetType.Employees);
        }

        if (IsRiskFindingDataset(fields))
        {
            candidates.Add(DatasetType.RiskFindings);
        }

        if (IsAccessAssignmentDataset(fields))
        {
            candidates.Add(DatasetType.AccessAssignments);
        }

        if (IsLoginActivityDataset(fields))
        {
            candidates.Add(DatasetType.LoginActivity);
        }

        // ---------------------------------------------------------
        // Role-Permission mapping
        //
        // This must be checked before Roles and Permissions because
        // it contains both RoleName and PermissionName.
        // ---------------------------------------------------------

        if (IsRolePermissionDataset(fields))
        {
            candidates.Add(DatasetType.RolePermissions);
        }

        if (IsRoleDataset(fields))
        {
            candidates.Add(DatasetType.Roles);
        }

        if (IsPermissionDataset(fields))
        {
            candidates.Add(DatasetType.Permissions);
        }

        if (IsDepartmentDataset(fields))
        {
            candidates.Add(DatasetType.Departments);
        }

        if (IsApplicationDataset(fields))
        {
            candidates.Add(DatasetType.Applications);
        }

        // ---------------------------------------------------------
        // No dataset detected
        // ---------------------------------------------------------

        if (candidates.Count == 0)
        {
            return new DatasetDetectionResult
            {
                DatasetType = DatasetType.Unknown,
                IsDetected = false,
                IsAmbiguous = false,
                Candidates = candidates,
                ErrorMessage =
                    "Unable to determine the dataset type from the supplied headers."
            };
        }

        // ---------------------------------------------------------
        // Multiple datasets detected
        // ---------------------------------------------------------

        if (candidates.Count > 1)
        {
            return new DatasetDetectionResult
            {
                DatasetType = DatasetType.Unknown,
                IsDetected = false,
                IsAmbiguous = true,
                Candidates = candidates,
                ErrorMessage =
                    "The supplied headers match multiple dataset types."
            };
        }

        // ---------------------------------------------------------
        // Exactly one dataset detected
        // ---------------------------------------------------------

        return new DatasetDetectionResult
        {
            DatasetType = candidates[0],
            IsDetected = true,
            IsAmbiguous = false,
            Candidates = candidates
        };
    }

    // =============================================================
    // Employees
    // =============================================================

    private static bool IsEmployeeDataset(
        HashSet<ImportField> fields)
    {
        return
            fields.Contains(ImportField.EmployeeId) &&
            fields.Contains(ImportField.EmployeeName) &&
            fields.Contains(ImportField.Email) &&
            fields.Contains(ImportField.Department);
    }

    // =============================================================
    // Departments
    // =============================================================

    private static bool IsDepartmentDataset(
        HashSet<ImportField> fields)
    {
        return
            fields.Contains(ImportField.Department) &&
            fields.Contains(ImportField.DepartmentCode) &&
            !fields.Contains(ImportField.EmployeeId) &&
            !fields.Contains(ImportField.ApplicationName) &&
            !fields.Contains(ImportField.RoleName) &&
            !fields.Contains(ImportField.PermissionName);
    }

    // =============================================================
    // Applications
    // =============================================================

    private static bool IsApplicationDataset(
        HashSet<ImportField> fields)
    {
        return
            fields.Contains(ImportField.ApplicationName) &&
            !fields.Contains(ImportField.EmployeeId) &&
            !fields.Contains(ImportField.RoleName) &&
            !fields.Contains(ImportField.PermissionName);
    }

    // =============================================================
    // Roles
    // =============================================================

    private static bool IsRoleDataset(
        HashSet<ImportField> fields)
    {
        return
            fields.Contains(ImportField.RoleName) &&
            fields.Contains(ImportField.ApplicationName) &&
            !fields.Contains(ImportField.EmployeeId) &&
            !fields.Contains(ImportField.PermissionName);
    }

    // =============================================================
    // Permissions
    // =============================================================

    private static bool IsPermissionDataset(
        HashSet<ImportField> fields)
    {
        return
            fields.Contains(ImportField.PermissionName) &&
            !fields.Contains(ImportField.EmployeeId) &&
            !fields.Contains(ImportField.RoleName);
    }

    // =============================================================
    // Role Permissions
    //
    // Example:
    //
    // Application | Role | Permission
    // Salesforce  | Admin | USER_CREATE
    // =============================================================

    private static bool IsRolePermissionDataset(
        HashSet<ImportField> fields)
    {
        return
            fields.Contains(ImportField.ApplicationName) &&
            fields.Contains(ImportField.RoleName) &&
            fields.Contains(ImportField.PermissionName) &&
            !fields.Contains(ImportField.EmployeeId);
    }

    // =============================================================
    // Risk Findings
    // =============================================================

    private static bool IsRiskFindingDataset(
        HashSet<ImportField> fields)
    {
        return
            fields.Contains(ImportField.EmployeeId) &&
            fields.Contains(ImportField.RuleCode) &&
            fields.Contains(ImportField.Description) &&
            fields.Contains(ImportField.RiskSeverity);
    }

    // =============================================================
    // Access Assignments
    // =============================================================

    private static bool IsAccessAssignmentDataset(
        HashSet<ImportField> fields)
    {
        return
            fields.Contains(ImportField.EmployeeId) &&
            fields.Contains(ImportField.ApplicationName) &&
            (
                fields.Contains(ImportField.RoleName) ||
                fields.Contains(ImportField.AccessStatus)
            );
    }

    // =============================================================
    // Login Activity
    // =============================================================

    private static bool IsLoginActivityDataset(
        HashSet<ImportField> fields)
    {
        return
            fields.Contains(ImportField.EmployeeId) &&
            fields.Contains(ImportField.LastLoginDate);
    }
}