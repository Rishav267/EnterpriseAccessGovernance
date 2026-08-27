namespace EnterpriseAccessGovernance.Application.Common.Models;

public sealed class RiskRuleSettings
{
    public int DormantAccountDays { get; init; } = 90;

    public int ExcessiveApplicationAccessCount { get; init; } = 5;
}