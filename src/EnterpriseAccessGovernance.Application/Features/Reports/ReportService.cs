using EnterpriseAccessGovernance.Application.Common.Interfaces;
using EnterpriseAccessGovernance.Application.Features.Reports.DTOs;

namespace EnterpriseAccessGovernance.Application.Features.Reports;

public sealed class ReportService : IReportService
{
    private readonly IReportRepository _repository;

    public ReportService(IReportRepository repository)
    {
        _repository =
            repository
            ?? throw new ArgumentNullException(
                nameof(repository));
    }

    public Task<IReadOnlyCollection<HighRiskUserDto>>
        GetHighRiskUsersAsync(
            CancellationToken cancellationToken = default)
    {
        return _repository.GetHighRiskUsersAsync(
            cancellationToken);
    }

    public Task<IReadOnlyCollection<DormantAccountDto>>
        GetDormantAccountsAsync(
            int dormantDays,
            CancellationToken cancellationToken = default)
    {
        if (dormantDays <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(dormantDays));
        }

        return _repository.GetDormantAccountsAsync(
            dormantDays,
            cancellationToken);
    }

    public Task<IReadOnlyCollection<AccessByDepartmentDto>>
        GetAccessByDepartmentAsync(
            CancellationToken cancellationToken = default)
    {
        return _repository.GetAccessByDepartmentAsync(
            cancellationToken);
    }

    public Task<IReadOnlyCollection<AccessByApplicationDto>>
        GetAccessByApplicationAsync(
            CancellationToken cancellationToken = default)
    {
        return _repository.GetAccessByApplicationAsync(
            cancellationToken);
    }

    public Task<IReadOnlyCollection<PendingCertificationDto>>
        GetPendingCertificationsAsync(
            CancellationToken cancellationToken = default)
    {
        return _repository.GetPendingCertificationsAsync(
            cancellationToken);
    }

    public Task<CertificationSummaryDto>
        GetCertificationSummaryAsync(
            CancellationToken cancellationToken = default)
    {
        return _repository.GetCertificationSummaryAsync(
            cancellationToken);
    }
}