namespace EnterpriseAccessGovernance.Application.Features.AccessAssignments.DTOs;

public sealed class AccessAssignmentListItemDto
{
    public Guid Id { get; init; }

    public Guid EmployeeId { get; init; }

    public Guid EnterpriseApplicationId { get; init; }

    public string ApplicationName { get; init; } = string.Empty;

    public string ApplicationCode { get; init; } = string.Empty;

    public Guid ApplicationRoleId { get; init; }

    public string RoleName { get; init; } = string.Empty;

    public string RoleCode { get; init; } = string.Empty;

    public bool IsHighPrivilege { get; init; }

    public string Status { get; init; } = string.Empty;

    public DateTime GrantedAtUtc { get; init; }

    public DateTime? ExpiresAtUtc { get; init; }

    public DateTime? RevokedAtUtc { get; init; }

    public DateTime? LastReviewedAtUtc { get; init; }
}