namespace EnterpriseAccessGovernance.Application.Features.RiskFindings.DTOs;

public sealed class RiskFindingQueryDto
{
    public string? Severity { get; init; }

    public string? Status { get; init; }

    public string? RuleCode { get; init; }

    public Guid? EmployeeId { get; init; }

    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = 20;

    public string? SearchTerm { get; init; }
}