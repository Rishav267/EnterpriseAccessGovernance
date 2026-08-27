using EnterpriseAccessGovernance.Application.Features.Dashboard.DTOs;

namespace EnterpriseAccessGovernance.Application.Common.Interfaces;

public interface IDashboardRepository
{
    Task<DashboardSummaryDto> GetSummaryAsync(
        CancellationToken cancellationToken = default);
}