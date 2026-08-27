using EnterpriseAccessGovernance.Application.Features.RiskFindings.DTOs;

namespace EnterpriseAccessGovernance.Application.Features.RiskFindings;

public interface IRiskFindingService
{
    Task<IReadOnlyCollection<RiskFindingListItemDto>>
        GetByEmployeeIdAsync(
            Guid employeeId,
            CancellationToken cancellationToken = default);

    Task<PagedRiskFindingResultDto>
        GetPagedAsync(
            RiskFindingQueryDto query,
            CancellationToken cancellationToken = default);

    Task<RiskFindingSummaryDto>
        GetSummaryAsync(
            CancellationToken cancellationToken = default);

    Task ResolveAsync(
        Guid employeeId,
        Guid riskFindingId,
        CancellationToken cancellationToken = default);

    Task IgnoreAsync(
        Guid employeeId,
        Guid riskFindingId,
        CancellationToken cancellationToken = default);
}