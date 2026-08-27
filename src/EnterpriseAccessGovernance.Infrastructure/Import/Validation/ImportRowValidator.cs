using System.Globalization;
using System.Net.Mail;
using EnterpriseAccessGovernance.Application.Common.Interfaces;
using EnterpriseAccessGovernance.Application.Common.Models;

namespace EnterpriseAccessGovernance.Infrastructure.Import.Validation;

public sealed class ImportRowValidator : IImportRowValidator
{
    public ImportRowValidationResult Validate(
        CanonicalImportRow row,
        DatasetType datasetType)
    {
        ArgumentNullException.ThrowIfNull(row);

        var errors = new List<string>();

        switch (datasetType)
        {
            case DatasetType.Employees:
                ValidateEmployee(row, errors);
                break;

            case DatasetType.AccessAssignments:
                ValidateAccessAssignment(row, errors);
                break;

            case DatasetType.LoginActivity:
                ValidateLoginActivity(row, errors);
                break;

            case DatasetType.RiskFindings:
                ValidateRiskFinding(row, errors);
                break;

            case DatasetType.Departments:
                ValidateDepartment(row, errors);
                break;

            case DatasetType.Applications:
                ValidateApplication(row, errors);
                break;

            case DatasetType.Roles:
                ValidateRole(row, errors);
                break;

            case DatasetType.Permissions:
                ValidatePermission(row, errors);
                break;

            default:
                errors.Add("Unsupported dataset type.");
                break;
        }

        return new ImportRowValidationResult
        {
            Errors = errors
        };
    }

    private static void ValidateEmployee(
        CanonicalImportRow row,
        ICollection<string> errors)
    {
        ValidateRequired(
            row,
            ImportField.EmployeeId,
            "EmployeeId",
            errors);

        ValidateRequired(
            row,
            ImportField.EmployeeName,
            "EmployeeName",
            errors);

        ValidateRequired(
            row,
            ImportField.Email,
            "Email",
            errors);

        ValidateRequired(
            row,
            ImportField.Department,
            "Department",
            errors);

        var email = row.Get(ImportField.Email);

        if (!string.IsNullOrWhiteSpace(email) &&
            !IsValidEmail(email))
        {
            errors.Add("Email is not valid.");
        }
    }

    private static void ValidateAccessAssignment(
    CanonicalImportRow row,
    ICollection<string> errors)
    {
        ValidateRequired(
            row,
            ImportField.EmployeeId,
            "EmployeeId",
            errors);

        ValidateRequired(
            row,
            ImportField.ApplicationName,
            "ApplicationName",
            errors);

        ValidateRequired(
            row,
            ImportField.RoleName,
            "RoleName",
            errors);

        var accessStatus =
            row.Get(ImportField.AccessStatus);

        if (!string.IsNullOrWhiteSpace(accessStatus) &&
            !IsValidAccessStatus(accessStatus))
        {
            errors.Add(
                "AccessStatus must be Active, PendingReview, or Revoked.");
        }

        ValidateDate(
            row,
            ImportField.AccessStartDate,
            "AccessStartDate",
            errors);

        ValidateDate(
            row,
            ImportField.AccessEndDate,
            "AccessEndDate",
            errors);
    }

    private static void ValidateLoginActivity(
        CanonicalImportRow row,
        ICollection<string> errors)
    {
        ValidateRequired(
            row,
            ImportField.EmployeeId,
            "EmployeeId",
            errors);

        ValidateDate(
            row,
            ImportField.LastLoginDate,
            "LastLoginDate",
            errors);
    }

    private static void ValidateRiskFinding(
    CanonicalImportRow row,
    ICollection<string> errors)
    {
        ValidateRequired(
            row,
            ImportField.EmployeeId,
            "EmployeeId",
            errors);

        ValidateRequired(
            row,
            ImportField.RuleCode,
            "RuleCode",
            errors);

        ValidateRequired(
            row,
            ImportField.Description,
            "Description",
            errors);

        ValidateRequired(
            row,
            ImportField.RiskSeverity,
            "RiskSeverity",
            errors);

        ValidateDate(
            row,
            ImportField.DetectedAtUtc,
            "DetectedAtUtc",
            errors);

        var severity =
            row.Get(ImportField.RiskSeverity);

        if (!string.IsNullOrWhiteSpace(severity) &&
            !IsValidRiskSeverity(severity))
        {
            errors.Add(
                "RiskSeverity must be Low, Medium, High, or Critical.");
        }
    }

    private static void ValidateDepartment(
        CanonicalImportRow row,
        ICollection<string> errors)
    {
        ValidateRequired(
            row,
            ImportField.Department,
            "Department",
            errors);

        ValidateRequired(
            row,
            ImportField.DepartmentCode,
            "DepartmentCode",
            errors);
    }

    private static void ValidateApplication(
        CanonicalImportRow row,
        ICollection<string> errors)
    {
        ValidateRequired(
            row,
            ImportField.ApplicationName,
            "ApplicationName",
            errors);
    }

    private static void ValidateRole(
    CanonicalImportRow row,
    ICollection<string> errors)
    {
        ValidateRequired(
            row,
            ImportField.ApplicationName,
            "ApplicationName",
            errors);

        ValidateRequired(
            row,
            ImportField.RoleName,
            "RoleName",
            errors);
    }

    private static void ValidatePermission(
        CanonicalImportRow row,
        ICollection<string> errors)
    {
        ValidateRequired(
            row,
            ImportField.PermissionName,
            "PermissionName",
            errors);
    }

    private static void ValidateRequired(
        CanonicalImportRow row,
        ImportField field,
        string displayName,
        ICollection<string> errors)
    {
        if (string.IsNullOrWhiteSpace(row.Get(field)))
        {
            errors.Add($"{displayName} is required.");
        }
    }

    private static void ValidateDate(
        CanonicalImportRow row,
        ImportField field,
        string displayName,
        ICollection<string> errors)
    {
        var value = row.Get(field);

        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        if (!DateTime.TryParse(
                value,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal |
                DateTimeStyles.AdjustToUniversal,
                out _))
        {
            errors.Add(
                $"{displayName} is not a valid date.");
        }
    }

    private static bool IsValidEmail(string value)
    {
        try
        {
            var address = new MailAddress(value);

            return string.Equals(
                address.Address,
                value,
                StringComparison.OrdinalIgnoreCase);
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private static bool IsValidRiskSeverity(
    string value)
    {
        return value.Equals(
                   "Low",
                   StringComparison.OrdinalIgnoreCase)
               || value.Equals(
                   "Medium",
                   StringComparison.OrdinalIgnoreCase)
               || value.Equals(
                   "High",
                   StringComparison.OrdinalIgnoreCase)
               || value.Equals(
                   "Critical",
                   StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsValidAccessStatus(
    string value)
    {
        return value.Equals(
                   "Active",
                   StringComparison.OrdinalIgnoreCase)
               || value.Equals(
                   "PendingReview",
                   StringComparison.OrdinalIgnoreCase)
               || value.Equals(
                   "Revoked",
                   StringComparison.OrdinalIgnoreCase);
    }
}