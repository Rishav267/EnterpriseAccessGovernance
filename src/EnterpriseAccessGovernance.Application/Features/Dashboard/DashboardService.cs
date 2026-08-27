using EnterpriseAccessGovernance.Application.Common.Interfaces;
using EnterpriseAccessGovernance.Application.Features.Dashboard.DTOs;

namespace EnterpriseAccessGovernance.Application.Features.Dashboard;

public sealed class DashboardService : IDashboardService
{
    private readonly IDashboardRepository _dashboardRepository;

    public DashboardService(
        IDashboardRepository dashboardRepository)
    {
        _dashboardRepository =
            dashboardRepository
            ?? throw new ArgumentNullException(
                nameof(dashboardRepository));
    }

    public Task<DashboardSummaryDto> GetSummaryAsync(
        CancellationToken cancellationToken = default)
    {
        return _dashboardRepository.GetSummaryAsync(
            cancellationToken);
    }

    public Task<IReadOnlyCollection<DashboardAccessDetailDto>>
        GetAccessDetailsAsync(
            string? employeeName,
            string? departmentName,
            string? applicationName,
            string? roleName,
            string? status,
            CancellationToken cancellationToken = default)
    {
        return _dashboardRepository.GetAccessDetailsAsync(
            employeeName,
            departmentName,
            applicationName,
            roleName,
            status,
            cancellationToken);
    }

    public Task<DashboardAccessSummaryDto>
        GetAccessSummaryAsync(
            CancellationToken cancellationToken = default)
    {
        return _dashboardRepository.GetAccessSummaryAsync(
            cancellationToken);
    }
}