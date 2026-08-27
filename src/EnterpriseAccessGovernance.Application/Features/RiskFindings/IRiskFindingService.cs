using EnterpriseAccessGovernance.Application.Features.RiskFindings.DTOs;

namespace EnterpriseAccessGovernance.Application.Features.RiskFindings;

public interface IRiskFindingService
{
    Task<IReadOnlyCollection<RiskFindingListItemDto>>
        GetByEmployeeIdAsync(
            Guid employeeId,
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