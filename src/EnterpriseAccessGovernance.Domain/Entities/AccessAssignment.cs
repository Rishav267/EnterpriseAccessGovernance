using EnterpriseAccessGovernance.Domain.Common;
using EnterpriseAccessGovernance.Domain.Enums;

namespace EnterpriseAccessGovernance.Domain.Entities;

public sealed class AccessAssignment : AuditableEntity
{
    private AccessAssignment()
    {
    }

    private AccessAssignment(
        Guid employeeId,
        Guid enterpriseApplicationId,
        Guid applicationRoleId,
        AccessStatus status,
        DateTime grantedAtUtc,
        DateTime? expiresAtUtc)
    {
        EmployeeId = employeeId;
        EnterpriseApplicationId = enterpriseApplicationId;
        ApplicationRoleId = applicationRoleId;
        Status = status;
        GrantedAtUtc = grantedAtUtc;
        ExpiresAtUtc = expiresAtUtc;
    }

    public Guid EmployeeId { get; private set; }

    public Guid EnterpriseApplicationId { get; private set; }

    public Guid ApplicationRoleId { get; private set; }

    public AccessStatus Status { get; private set; }

    public DateTime GrantedAtUtc { get; private set; }

    public DateTime? ExpiresAtUtc { get; private set; }

    public DateTime? RevokedAtUtc { get; private set; }

    public DateTime? LastReviewedAtUtc { get; private set; }

    public Employee? Employee { get; private set; }

    public EnterpriseApplication? EnterpriseApplication { get; private set; }

    public ApplicationRole? ApplicationRole { get; private set; }

    public static AccessAssignment Create(
        Guid employeeId,
        Guid enterpriseApplicationId,
        Guid applicationRoleId,
        DateTime grantedAtUtc,
        DateTime? expiresAtUtc = null)
    {
        if (employeeId == Guid.Empty)
        {
            throw new ArgumentException(
                "Employee is required.",
                nameof(employeeId));
        }

        if (enterpriseApplicationId == Guid.Empty)
        {
            throw new ArgumentException(
                "Application is required.",
                nameof(enterpriseApplicationId));
        }

        if (applicationRoleId == Guid.Empty)
        {
            throw new ArgumentException(
                "Application role is required.",
                nameof(applicationRoleId));
        }

        if (expiresAtUtc.HasValue &&
            expiresAtUtc.Value <= grantedAtUtc)
        {
            throw new ArgumentException(
                "Expiration must be after the grant date.",
                nameof(expiresAtUtc));
        }

        return new AccessAssignment(
            employeeId,
            enterpriseApplicationId,
            applicationRoleId,
            AccessStatus.Active,
            grantedAtUtc,
            expiresAtUtc);
    }

    public void StartCertification()
    {
        if (Status is AccessStatus.Revoked or AccessStatus.Expired)
        {
            throw new InvalidOperationException(
                "Revoked or expired access cannot enter certification.");
        }

        Status = AccessStatus.PendingReview;
        MarkUpdated();
    }

    public void ApproveCertification(DateTime reviewedAtUtc)
    {
        EnsureReviewable();

        Status = AccessStatus.Active;
        LastReviewedAtUtc = reviewedAtUtc;
        MarkUpdated();
    }

    public void Revoke(DateTime revokedAtUtc)
    {
        if (Status == AccessStatus.Revoked)
        {
            throw new InvalidOperationException(
                "Access has already been revoked.");
        }

        Status = AccessStatus.Revoked;
        RevokedAtUtc = revokedAtUtc;
        LastReviewedAtUtc = revokedAtUtc;
        MarkUpdated();
    }

    public void RequestModification(DateTime reviewedAtUtc)
    {
        EnsureReviewable();

        Status = AccessStatus.ModificationRequested;
        LastReviewedAtUtc = reviewedAtUtc;
        MarkUpdated();
    }

    public void MarkExpired(DateTime currentUtc)
    {
        var isExpired =
            ExpiresAtUtc.HasValue &&
            ExpiresAtUtc.Value <= currentUtc;

        var canExpire =
            Status is AccessStatus.Active
                or AccessStatus.PendingReview
                or AccessStatus.ModificationRequested;

        if (isExpired && canExpire)
        {
            Status = AccessStatus.Expired;
            MarkUpdated();
        }
    }

    private void EnsureReviewable()
    {
        if (Status is AccessStatus.Revoked or AccessStatus.Expired)
        {
            throw new InvalidOperationException(
                "This access assignment cannot be reviewed.");
        }
    }
}