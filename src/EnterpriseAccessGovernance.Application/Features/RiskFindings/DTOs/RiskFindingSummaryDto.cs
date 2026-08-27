namespace EnterpriseAccessGovernance.Application.Features.RiskFindings.DTOs;

public sealed class RiskFindingSummaryDto
{
    public int TotalOpen { get; init; }

    public int Critical { get; init; }

    public int High { get; init; }

    public int Medium { get; init; }

    public int Low { get; init; }

    public int TotalResolved { get; init; }

    public int TotalIgnored { get; init; }
}