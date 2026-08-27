using EnterpriseAccessGovernance.Application.Features.RiskFindings.DTOs;
using EnterpriseAccessGovernance.Domain.Entities;

namespace EnterpriseAccessGovernance.Application.Common.Interfaces;

public interface IRiskDetectionRepository
{
    Task<IReadOnlyCollection<RiskDetectionEmployeeDto>>
        GetDetectionDataAsync(
            CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<RiskFinding>>
        GetOpenFindingsAsync(
            CancellationToken cancellationToken = default);

    Task AddFindingAsync(
        RiskFinding finding,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}