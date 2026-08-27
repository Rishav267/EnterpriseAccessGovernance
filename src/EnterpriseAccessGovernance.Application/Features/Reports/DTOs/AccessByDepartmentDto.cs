namespace EnterpriseAccessGovernance.Application.Features.Reports.DTOs;

public sealed class AccessByDepartmentDto
{
    public Guid? DepartmentId { get; init; }

    public string DepartmentName { get; init; } = string.Empty;

    public int EmployeeCount { get; init; }

    public int AccessAssignmentCount { get; init; }

    public int ActiveAccessAssignmentCount { get; init; }
}