using EnterpriseAccessGovernance.Application.Features.RiskFindings.DTOs;
using EnterpriseAccessGovernance.Domain.Entities;

namespace EnterpriseAccessGovernance.Application.Common.Interfaces;

public interface IRiskFindingRepository
{
    Task<IReadOnlyCollection<RiskFindingListItemDto>>
        GetByEmployeeIdAsync(
            Guid employeeId,
            CancellationToken cancellationToken = default);

    Task<RiskFinding?> GetByIdAsync(
        Guid riskFindingId,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}