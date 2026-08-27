namespace EnterpriseAccessGovernance.Application.Features.Reports.DTOs;

public sealed class PendingCertificationDto
{
    public Guid AccessAssignmentId { get; init; }

    public Guid EmployeeId { get; init; }

    public string EmployeeName { get; init; } = string.Empty;

    public string DepartmentName { get; init; } = string.Empty;

    public string ApplicationName { get; init; } = string.Empty;

    public string RoleName { get; init; } = string.Empty;

    public DateTime GrantedAtUtc { get; init; }

    public DateTime? LastReviewedAtUtc { get; init; }
}