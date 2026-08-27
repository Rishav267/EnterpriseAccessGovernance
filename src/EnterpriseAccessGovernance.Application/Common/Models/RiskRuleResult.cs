using EnterpriseAccessGovernance.Domain.Enums;

namespace EnterpriseAccessGovernance.Application.Common.Models;

public sealed class RiskRuleResult
{
    public Guid EmployeeId { get; init; }

    public string RuleCode { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public RiskSeverity Severity { get; init; }
}