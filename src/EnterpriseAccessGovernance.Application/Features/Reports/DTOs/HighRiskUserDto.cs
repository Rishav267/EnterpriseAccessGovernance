namespace EnterpriseAccessGovernance.Application.Features.Reports.DTOs;

public sealed class HighRiskUserDto
{
    public Guid EmployeeId { get; init; }

    public string EmployeeNumber { get; init; } = string.Empty;

    public string EmployeeName { get; init; } = string.Empty;

    public string DepartmentName { get; init; } = string.Empty;

    public int RiskFindingCount { get; init; }

    public string HighestSeverity { get; init; } = string.Empty;
}