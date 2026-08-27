using EnterpriseAccessGovernance.Application.Common.Interfaces;
using EnterpriseAccessGovernance.Application.Features.AccessAssignments.DTOs;
using EnterpriseAccessGovernance.Domain.Entities;
using EnterpriseAccessGovernance.Domain.Enums;

namespace EnterpriseAccessGovernance.Application.Features.AccessAssignments;

public sealed class AccessAssignmentService
    : IAccessAssignmentService
{
    private readonly IAccessAssignmentRepository
        _accessAssignmentRepository;

    public AccessAssignmentService(
        IAccessAssignmentRepository accessAssignmentRepository)
    {
        _accessAssignmentRepository =
            accessAssignmentRepository
            ?? throw new ArgumentNullException(
                nameof(accessAssignmentRepository));
    }

    public Task<IReadOnlyCollection<AccessAssignmentListItemDto>>
        GetByEmployeeIdAsync(
            Guid employeeId,
            CancellationToken cancellationToken = default)
    {
        ValidateEmployeeId(employeeId);

        return _accessAssignmentRepository
            .GetByEmployeeIdAsync(
                employeeId,
                cancellationToken);
    }

    public async Task ApproveAsync(
        Guid employeeId,
        Guid accessAssignmentId,
        Guid reviewerEmployeeId,
        string? comment,
        CancellationToken cancellationToken = default)
    {
        ValidateIds(
            employeeId,
            accessAssignmentId,
            reviewerEmployeeId);

        var assignment =
            await GetAssignmentAsync(
                employeeId,
                accessAssignmentId,
                cancellationToken);

        var reviewedAtUtc = DateTime.UtcNow;

        assignment.ApproveCertification(
            reviewedAtUtc);

        var review =
            CertificationReview.Create(
                accessAssignmentId,
                reviewerEmployeeId,
                CertificationDecision.Approved,
                comment,
                reviewedAtUtc);

        await _accessAssignmentRepository
            .AddCertificationReviewAsync(
                review,
                cancellationToken);

        var auditLog =
            AuditLog.Create(
                reviewerEmployeeId,
                AuditAction.AccessApproved,
                nameof(AccessAssignment),
                accessAssignmentId,
                comment,
                reviewedAtUtc);

        await _accessAssignmentRepository
            .AddAuditLogAsync(
                auditLog,
                cancellationToken);

        await _accessAssignmentRepository
            .SaveChangesAsync(
                cancellationToken);
    }

    public async Task RevokeAsync(
        Guid employeeId,
        Guid accessAssignmentId,
        Guid reviewerEmployeeId,
        string? comment,
        CancellationToken cancellationToken = default)
    {
        ValidateIds(
            employeeId,
            accessAssignmentId,
            reviewerEmployeeId);

        var assignment =
            await GetAssignmentAsync(
                employeeId,
                accessAssignmentId,
                cancellationToken);

        var reviewedAtUtc = DateTime.UtcNow;

        assignment.Revoke(
            reviewedAtUtc);

        var review =
            CertificationReview.Create(
                accessAssignmentId,
                reviewerEmployeeId,
                CertificationDecision.Revoked,
                comment,
                reviewedAtUtc);

        await _accessAssignmentRepository
            .AddCertificationReviewAsync(
                review,
                cancellationToken);

        var auditLog =
            AuditLog.Create(
                reviewerEmployeeId,
                AuditAction.AccessRevoked,
                nameof(AccessAssignment),
                accessAssignmentId,
                comment,
                reviewedAtUtc);

        await _accessAssignmentRepository
            .AddAuditLogAsync(
                auditLog,
                cancellationToken);

        await _accessAssignmentRepository
            .SaveChangesAsync(
                cancellationToken);
    }

    public async Task RequestModificationAsync(
        Guid employeeId,
        Guid accessAssignmentId,
        Guid reviewerEmployeeId,
        string comment,
        CancellationToken cancellationToken = default)
    {
        ValidateIds(
            employeeId,
            accessAssignmentId,
            reviewerEmployeeId);

        if (string.IsNullOrWhiteSpace(comment))
        {
            throw new ArgumentException(
                "Modification comment is required.",
                nameof(comment));
        }

        var assignment =
            await GetAssignmentAsync(
                employeeId,
                accessAssignmentId,
                cancellationToken);

        var reviewedAtUtc = DateTime.UtcNow;

        assignment.RequestModification(
            reviewedAtUtc);

        var review =
            CertificationReview.Create(
                accessAssignmentId,
                reviewerEmployeeId,
                CertificationDecision.ModificationRequested,
                comment,
                reviewedAtUtc);

        await _accessAssignmentRepository
            .AddCertificationReviewAsync(
                review,
                cancellationToken);

        var auditLog =
            AuditLog.Create(
                reviewerEmployeeId,
                AuditAction.ModificationRequested,
                nameof(AccessAssignment),
                accessAssignmentId,
                comment,
                reviewedAtUtc);

        await _accessAssignmentRepository
            .AddAuditLogAsync(
                auditLog,
                cancellationToken);

        await _accessAssignmentRepository
            .SaveChangesAsync(
                cancellationToken);
    }

    public async Task<IReadOnlyCollection<CertificationReviewDto>>
        GetCertificationHistoryAsync(
            Guid employeeId,
            Guid accessAssignmentId,
            CancellationToken cancellationToken = default)
    {
        ValidateEmployeeId(employeeId);

        if (accessAssignmentId == Guid.Empty)
        {
            throw new ArgumentException(
                "Access assignment ID is required.",
                nameof(accessAssignmentId));
        }

        await GetAssignmentAsync(
            employeeId,
            accessAssignmentId,
            cancellationToken);

        return await _accessAssignmentRepository
            .GetCertificationHistoryAsync(
                employeeId,
                accessAssignmentId,
                cancellationToken);
    }

    private async Task<AccessAssignment>
        GetAssignmentAsync(
            Guid employeeId,
            Guid accessAssignmentId,
            CancellationToken cancellationToken)
    {
        var assignment =
            await _accessAssignmentRepository
                .GetByIdAsync(
                    accessAssignmentId,
                    cancellationToken);

        if (assignment is null ||
            assignment.EmployeeId != employeeId)
        {
            throw new KeyNotFoundException(
                "Access assignment was not found for this employee.");
        }

        return assignment;
    }

    private static void ValidateEmployeeId(
        Guid employeeId)
    {
        if (employeeId == Guid.Empty)
        {
            throw new ArgumentException(
                "Employee ID is required.",
                nameof(employeeId));
        }
    }

    private static void ValidateIds(
        Guid employeeId,
        Guid accessAssignmentId,
        Guid reviewerEmployeeId)
    {
        ValidateEmployeeId(employeeId);

        if (accessAssignmentId == Guid.Empty)
        {
            throw new ArgumentException(
                "Access assignment ID is required.",
                nameof(accessAssignmentId));
        }

        if (reviewerEmployeeId == Guid.Empty)
        {
            throw new ArgumentException(
                "Reviewer employee ID is required.",
                nameof(reviewerEmployeeId));
        }
    }
}