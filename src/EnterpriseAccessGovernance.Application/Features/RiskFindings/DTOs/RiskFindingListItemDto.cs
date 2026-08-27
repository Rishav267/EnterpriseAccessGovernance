namespace EnterpriseAccessGovernance.Application.Features.RiskFindings.DTOs;

public sealed class RiskFindingListItemDto
{
    public Guid Id { get; init; }

    public Guid EmployeeId { get; init; }

    public string EmployeeName { get; init; } = string.Empty;

    public string RuleCode { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public string Severity { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public DateTime DetectedAtUtc { get; init; }

    public DateTime? ResolvedAtUtc { get; init; }
}