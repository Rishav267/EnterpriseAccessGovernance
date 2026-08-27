using EnterpriseAccessGovernance.Domain.Common;
using EnterpriseAccessGovernance.Domain.Enums;

namespace EnterpriseAccessGovernance.Domain.Entities;

public sealed class CertificationReview : AuditableEntity
{
    private CertificationReview()
    {
    }

    private CertificationReview(
        Guid accessAssignmentId,
        Guid reviewerEmployeeId,
        CertificationDecision decision,
        string? comment,
        DateTime reviewedAtUtc)
    {
        AccessAssignmentId = accessAssignmentId;
        ReviewerEmployeeId = reviewerEmployeeId;
        Decision = decision;
        Comment = comment;
        ReviewedAtUtc = reviewedAtUtc;
    }

    public Guid AccessAssignmentId { get; private set; }

    public Guid ReviewerEmployeeId { get; private set; }

    public CertificationDecision Decision { get; private set; }

    public string? Comment { get; private set; }

    public DateTime ReviewedAtUtc { get; private set; }

    public AccessAssignment? AccessAssignment { get; private set; }

    public Employee? ReviewerEmployee { get; private set; }

    public static CertificationReview Create(
        Guid accessAssignmentId,
        Guid reviewerEmployeeId,
        CertificationDecision decision,
        string? comment,
        DateTime reviewedAtUtc)
    {
        if (accessAssignmentId == Guid.Empty)
        {
            throw new ArgumentException(
                "Access assignment is required.",
                nameof(accessAssignmentId));
        }

        if (reviewerEmployeeId == Guid.Empty)
        {
            throw new ArgumentException(
                "Reviewer is required.",
                nameof(reviewerEmployeeId));
        }

        return new CertificationReview(
            accessAssignmentId,
            reviewerEmployeeId,
            decision,
            string.IsNullOrWhiteSpace(comment)
                ? null
                : comment.Trim(),
            reviewedAtUtc);
    }
}