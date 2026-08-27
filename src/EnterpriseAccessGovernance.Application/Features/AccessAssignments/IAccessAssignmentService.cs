using EnterpriseAccessGovernance.Application.Features.AccessAssignments.DTOs;

namespace EnterpriseAccessGovernance.Application.Features.AccessAssignments;

public interface IAccessAssignmentService
{
    Task<IReadOnlyCollection<AccessAssignmentListItemDto>>
        GetByEmployeeIdAsync(
            Guid employeeId,
            CancellationToken cancellationToken = default);

    Task ApproveAsync(
        Guid employeeId,
        Guid accessAssignmentId,
        Guid reviewerEmployeeId,
        string? comment,
        CancellationToken cancellationToken = default);

    Task RevokeAsync(
        Guid employeeId,
        Guid accessAssignmentId,
        Guid reviewerEmployeeId,
        string? comment,
        CancellationToken cancellationToken = default);

    Task RequestModificationAsync(
        Guid employeeId,
        Guid accessAssignmentId,
        Guid reviewerEmployeeId,
        string comment,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<CertificationReviewDto>>
        GetCertificationHistoryAsync(
            Guid employeeId,
            Guid accessAssignmentId,
            CancellationToken cancellationToken = default);
}