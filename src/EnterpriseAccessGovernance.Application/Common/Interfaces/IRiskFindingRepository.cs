using EnterpriseAccessGovernance.Application.Features.RiskFindings.DTOs;
using EnterpriseAccessGovernance.Domain.Entities;

namespace EnterpriseAccessGovernance.Application.Common.Interfaces;

public interface IRiskFindingRepository
{
    Task<IReadOnlyCollection<RiskFindingListItemDto>>
        GetByEmployeeIdAsync(
            Guid employeeId,
            CancellationToken cancellationToken = default);

    Task<RiskFinding?>
        GetByIdAsync(
            Guid riskFindingId,
            CancellationToken cancellationToken = default);

    Task<(IReadOnlyCollection<RiskFindingListItemDto> Items, int TotalCount)>
        GetPagedAsync(
            RiskFindingQueryDto query,
            CancellationToken cancellationToken = default);

    Task<RiskFindingSummaryDto>
        GetSummaryAsync(
            CancellationToken cancellationToken = default);

    Task<bool>
        ExistsOpenFindingAsync(
            Guid employeeId,
            string ruleCode,
            CancellationToken cancellationToken = default);

    Task AddAsync(
        RiskFinding riskFinding,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}