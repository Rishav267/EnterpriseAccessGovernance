namespace EnterpriseAccessGovernance.Application.Features.RiskFindings.DTOs;

public sealed class RiskDetectionResultDto
{
    public int TotalFindings { get; init; }

    public int InactiveEmployeeFindings { get; init; }

    public int DormantAccountFindings { get; init; }

    public int HighPrivilegeFindings { get; init; }

    public int ExpiredAccessFindings { get; init; }

    public int ExcessiveAccessFindings { get; init; }
}