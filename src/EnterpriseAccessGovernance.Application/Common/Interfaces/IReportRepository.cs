using EnterpriseAccessGovernance.Application.Features.Reports.DTOs;

namespace EnterpriseAccessGovernance.Application.Common.Interfaces;

public interface IReportRepository
{
    Task<IReadOnlyCollection<HighRiskUserDto>>
        GetHighRiskUsersAsync(
            CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<DormantAccountDto>>
        GetDormantAccountsAsync(
            int dormantDays,
            CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<AccessByDepartmentDto>>
        GetAccessByDepartmentAsync(
            CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<AccessByApplicationDto>>
        GetAccessByApplicationAsync(
            CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<PendingCertificationDto>>
        GetPendingCertificationsAsync(
            CancellationToken cancellationToken = default);

    Task<CertificationSummaryDto>
        GetCertificationSummaryAsync(
            CancellationToken cancellationToken = default);
}