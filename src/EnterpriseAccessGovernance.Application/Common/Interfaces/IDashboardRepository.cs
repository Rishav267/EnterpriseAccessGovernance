using EnterpriseAccessGovernance.Application.Features.Dashboard.DTOs;

namespace EnterpriseAccessGovernance.Application.Common.Interfaces;

public interface IDashboardRepository
{
    Task<DashboardSummaryDto> GetSummaryAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<DashboardAccessDetailDto>>
        GetAccessDetailsAsync(
            string? employeeName,
            string? departmentName,
            string? applicationName,
            string? roleName,
            string? status,
            CancellationToken cancellationToken = default);

    Task<DashboardAccessSummaryDto>
        GetAccessSummaryAsync(
            CancellationToken cancellationToken = default);
}