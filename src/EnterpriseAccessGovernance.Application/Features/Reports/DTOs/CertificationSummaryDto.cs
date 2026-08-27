namespace EnterpriseAccessGovernance.Application.Features.Reports.DTOs;

public sealed class CertificationSummaryDto
{
    public int TotalAssignments { get; init; }

    public int ReviewedAssignments { get; init; }

    public int PendingAssignments { get; init; }

    public int ApprovedReviews { get; init; }

    public int RevokedReviews { get; init; }

    public int ModificationRequests { get; init; }

    public decimal CompletionPercentage { get; init; }
}