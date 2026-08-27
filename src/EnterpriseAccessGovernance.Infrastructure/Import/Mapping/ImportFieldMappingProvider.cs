using EnterpriseAccessGovernance.Application.Common.Interfaces;
using EnterpriseAccessGovernance.Application.Common.Models;

namespace EnterpriseAccessGovernance.Infrastructure.Import.Mapping;

public sealed class ImportFieldMappingProvider
    : IImportFieldMappingProvider
{
    private static readonly IReadOnlyCollection<ImportFieldDefinition>
        Definitions =
        [
            new()
            {
                Field = ImportField.EmployeeId,
                Required = false,
                Aliases =
                [
                    "employeeid",
                    "employeenumber",
                    "employeeidentifier"
                ]
            },

            new()
            {
                Field = ImportField.EmployeeName,
                Required = false,
                Aliases =
                [
                    "employeename",
                    "fullname",
                    "name"
                ]
            },

            new()
            {
                Field = ImportField.Email,
                Required = false,
                Aliases =
                [
                    "email",
                    "emailaddress",
                    "emailid"
                ]
            },

            new()
            {
                Field = ImportField.Department,
                Required = false,
                Aliases =
                [
                    "department",
                    "departmentname",
                    "dept"
                ]
            },

            new()
            {
                Field = ImportField.DepartmentCode,
                Required = false,
                Aliases =
                [
                    "departmentcode",
                    "deptcode"
                ]
            },

            new()
            {
                Field = ImportField.ManagerId,
                Required = false,
                Aliases =
                [
                    "managerid",
                    "manageremployeeid",
                    "manageremployee"
                ]
            },

            new()
            {
                Field = ImportField.ManagerEmail,
                Required = false,
                Aliases =
                [
                    "manageremail",
                    "manageremailaddress"
                ]
            },

            new()
            {
                Field = ImportField.ManagerName,
                Required = false,
                Aliases =
                [
                    "manager",
                    "managername"
                ]
            },

            new()
            {
                Field = ImportField.ApplicationId,
                Required = false,
                Aliases =
                [
                    "applicationid",
                    "appid",
                    "applicationidentifier"
                ]
            },

            new()
            {
                Field = ImportField.ApplicationName,
                Required = false,
                Aliases =
                [
                    "application",
                    "applicationname",
                    "app",
                    "appname"
                ]
            },

            new()
            {
                Field = ImportField.RoleId,
                Required = false,
                Aliases =
                [
                    "roleid",
                    "roleidentifier"
                ]
            },

            new()
            {
                Field = ImportField.RoleName,
                Required = false,
                Aliases =
                [
                    "role",
                    "rolename"
                ]
            },

            new()
            {
                Field = ImportField.PermissionId,
                Required = false,
                Aliases =
                [
                    "permissionid",
                    "permissionidentifier"
                ]
            },

            new()
            {
                Field = ImportField.PermissionName,
                Required = false,
                Aliases =
                [
                    "permission",
                    "permissionname"
                ]
            },

            new()
            {
                Field = ImportField.AccessStatus,
                Required = false,
                Aliases =
                [
                    "accessstatus",
                    "status",
                    "assignmentstatus"
                ]
            },

            new()
            {
                Field = ImportField.AccessStartDate,
                Required = false,
                Aliases =
                [
                    "accessstartdate",
                    "startdate",
                    "start"
                ]
            },

            new()
            {
                Field = ImportField.AccessEndDate,
                Required = false,
                Aliases =
                [
                    "accessenddate",
                    "enddate",
                    "expirydate",
                    "expirationdate"
                ]
            },

            new()
            {
                Field = ImportField.LastLoginDate,
                Required = false,
                Aliases =
                [
                    "lastlogin",
                    "lastlogindate",
                    "lastloginat"
                ]
            },

            new()
            {
                Field = ImportField.LoginStatus,
                Required = false,
                Aliases =
                [
                    "loginstatus",
                    "accountstatus"
                ]
            },

            new()
            {
                Field = ImportField.RuleCode,
                Required = false,
                Aliases =
                [
                    "rulecode",
                    "rule",
                    "riskrule",
                    "riskrulecode"
                ]
            },

            new()
            {
                Field = ImportField.Description,
                Required = false,
                Aliases =
                [
                    "description",
                    "riskdescription",
                    "findingdescription"
                ]
            },

            new()
            {
                Field = ImportField.RiskSeverity,
                Required = false,
                Aliases =
                [
                    "riskseverity",
                    "severity",
                    "risklevel"
                ]
            },

            new()
            {
                Field = ImportField.DetectedAtUtc,
                Required = false,
                Aliases =
                [
                    "detectedatutc",
                    "detectedat",
                    "detectiondate",
                    "detecteddate"
                ]
            }
        ];

    public IReadOnlyCollection<ImportFieldDefinition> GetDefinitions()
    {
        return Definitions;
    }
}