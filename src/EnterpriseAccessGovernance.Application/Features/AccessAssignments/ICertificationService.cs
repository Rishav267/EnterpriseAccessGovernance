using EnterpriseAccessGovernance.Application.Features.AccessAssignments.DTOs;

namespace EnterpriseAccessGovernance.Application.Features.AccessAssignments;

public interface ICertificationService
{
    Task<CertificationReviewDto> ReviewAsync(
        CertificationRequestDto request,
        CancellationToken cancellationToken = default);
}