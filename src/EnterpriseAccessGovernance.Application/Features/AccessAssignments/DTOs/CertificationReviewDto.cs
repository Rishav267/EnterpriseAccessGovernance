namespace EnterpriseAccessGovernance.Application.Features.AccessAssignments.DTOs;

public sealed class CertificationReviewDto
{
    public Guid Id { get; init; }

    public Guid AccessAssignmentId { get; init; }

    public Guid ReviewerEmployeeId { get; init; }

    public string Decision { get; init; } = string.Empty;

    public string? Comment { get; init; }

    public DateTime ReviewedAtUtc { get; init; }
}