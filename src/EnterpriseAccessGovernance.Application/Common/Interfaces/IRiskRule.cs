using EnterpriseAccessGovernance.Application.Common.Models;

namespace EnterpriseAccessGovernance.Application.Common.Interfaces;

public interface IRiskRule
{
    string RuleCode { get; }

    Task<IReadOnlyCollection<RiskRuleResult>> EvaluateAsync(
        CancellationToken cancellationToken = default);
}