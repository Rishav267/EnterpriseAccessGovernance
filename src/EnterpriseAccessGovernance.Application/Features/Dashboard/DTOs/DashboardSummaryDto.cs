namespace EnterpriseAccessGovernance.Application.Features.Dashboard.DTOs;

public sealed class DashboardSummaryDto
{
    public int TotalEmployees { get; init; }

    public int ActiveEmployees { get; init; }

    public int InactiveEmployees { get; init; }

    public int TotalApplications { get; init; }

    public int TotalAccessAssignments { get; init; }

    public int ActiveAccessAssignments { get; init; }

    public int PendingReviews { get; init; }

    public int HighRiskUsers { get; init; }

    public int OpenRiskFindings { get; init; }
}