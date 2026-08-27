namespace EnterpriseAccessGovernance.Application.Features.Dashboard.DTOs;

public sealed class DashboardAccessDetailDto
{
    public Guid EmployeeId { get; init; }

    public string EmployeeName { get; init; } = string.Empty;

    public string DepartmentName { get; init; } = string.Empty;

    public string ApplicationName { get; init; } = string.Empty;

    public string RoleName { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public bool IsHighPrivilege { get; init; }

    public DateTime? ExpiresAtUtc { get; init; }
}