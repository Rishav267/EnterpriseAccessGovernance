namespace EnterpriseAccessGovernance.Application.Features.Reports.DTOs;

public sealed class DormantAccountDto
{
    public Guid EmployeeId { get; init; }

    public string EmployeeNumber { get; init; } = string.Empty;

    public string EmployeeName { get; init; } = string.Empty;

    public string DepartmentName { get; init; } = string.Empty;

    public DateTime? LastLoginAtUtc { get; init; }

    public int ActiveAccessCount { get; init; }
}