using EnterpriseAccessGovernance.Domain.Common;

namespace EnterpriseAccessGovernance.Domain.Entities;

public sealed class CertificationReviewAudit : AuditableEntity
{
    private CertificationReviewAudit()
    {
    }

    private CertificationReviewAudit(
        Guid certificationReviewId,
        Guid accessAssignmentId,
        Guid reviewerEmployeeId,
        string action,
        string? comment,
        DateTime actionAtUtc)
    {
        CertificationReviewId = certificationReviewId;
        AccessAssignmentId = accessAssignmentId;
        ReviewerEmployeeId = reviewerEmployeeId;
        Action = action;
        Comment = comment;
        ActionAtUtc = actionAtUtc;
    }

    public Guid CertificationReviewId { get; private set; }

    public Guid AccessAssignmentId { get; private set; }

    public Guid ReviewerEmployeeId { get; private set; }

    public string Action { get; private set; } = string.Empty;

    public string? Comment { get; private set; }

    public DateTime ActionAtUtc { get; private set; }

    public CertificationReview? CertificationReview { get; private set; }

    public AccessAssignment? AccessAssignment { get; private set; }

    public Employee? ReviewerEmployee { get; private set; }

    public static CertificationReviewAudit Create(
        Guid certificationReviewId,
        Guid accessAssignmentId,
        Guid reviewerEmployeeId,
        string action,
        string? comment,
        DateTime actionAtUtc)
    {
        if (certificationReviewId == Guid.Empty)
        {
            throw new ArgumentException(
                "Certification review is required.",
                nameof(certificationReviewId));
        }

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

        if (string.IsNullOrWhiteSpace(action))
        {
            throw new ArgumentException(
                "Audit action is required.",
                nameof(action));
        }

        return new CertificationReviewAudit(
            certificationReviewId,
            accessAssignmentId,
            reviewerEmployeeId,
            action.Trim(),
            string.IsNullOrWhiteSpace(comment)
                ? null
                : comment.Trim(),
            actionAtUtc);
    }
}