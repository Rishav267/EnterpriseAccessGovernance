using EnterpriseAccessGovernance.Application.Common.Interfaces;
using EnterpriseAccessGovernance.Application.Features.Dashboard.DTOs;

namespace EnterpriseAccessGovernance.Application.Features.Dashboard;

public sealed class DashboardService : IDashboardService
{
    private readonly IDashboardRepository _dashboardRepository;

    public DashboardService(
        IDashboardRepository dashboardRepository)
    {
        _dashboardRepository = dashboardRepository;
    }

    public Task<DashboardSummaryDto> GetSummaryAsync(
        CancellationToken cancellationToken = default)
    {
        return _dashboardRepository.GetSummaryAsync(
            cancellationToken);
    }
}