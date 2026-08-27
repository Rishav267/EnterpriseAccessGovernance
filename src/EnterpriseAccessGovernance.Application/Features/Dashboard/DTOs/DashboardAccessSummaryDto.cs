namespace EnterpriseAccessGovernance.Application.Features.Dashboard.DTOs;

public sealed class DashboardAccessSummaryDto
{
    public IReadOnlyCollection<DashboardAccessStatusDto>
        AccessByStatus
    { get; init; }
        = Array.Empty<DashboardAccessStatusDto>();

    public IReadOnlyCollection<DashboardApplicationAccessDto>
        AccessByApplication
    { get; init; }
        = Array.Empty<DashboardApplicationAccessDto>();

    public int HighRiskUsers { get; init; }

    public int NormalUsers { get; init; }
}

public sealed class DashboardAccessStatusDto
{
    public string Status { get; init; } = string.Empty;

    public int Count { get; init; }
}

public sealed class DashboardApplicationAccessDto
{
    public string ApplicationName { get; init; } = string.Empty;

    public int Count { get; init; }
}