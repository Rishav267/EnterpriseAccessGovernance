namespace EnterpriseAccessGovernance.Application.Features.RiskFindings.DTOs;

public sealed class RiskDetectionEmployeeDto
{
    public Guid EmployeeId { get; init; }

    public string EmployeeName { get; init; } = string.Empty;

    public string EmploymentStatus { get; init; } = string.Empty;

    public DateTime? LastLoginAtUtc { get; init; }

    public IReadOnlyCollection<RiskDetectionAccessDto> AccessAssignments { get; init; }
        = Array.Empty<RiskDetectionAccessDto>();
}

public sealed class RiskDetectionAccessDto
{
    public Guid AccessAssignmentId { get; init; }

    public Guid EnterpriseApplicationId { get; init; }

    public string ApplicationName { get; init; } = string.Empty;

    public string RoleName { get; init; } = string.Empty;

    public bool IsHighPrivilege { get; init; }

    public string Status { get; init; } = string.Empty;

    public DateTime? ExpiresAtUtc { get; init; }
}
