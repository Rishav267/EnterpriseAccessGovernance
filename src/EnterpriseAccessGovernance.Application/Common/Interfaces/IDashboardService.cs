using EnterpriseAccessGovernance.Application.Features.Dashboard.DTOs;

namespace EnterpriseAccessGovernance.Application.Common.Interfaces;

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetSummaryAsync(
        CancellationToken cancellationToken = default);
}