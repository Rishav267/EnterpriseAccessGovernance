namespace EnterpriseAccessGovernance.Application.Features.ApplicationRoles.DTOs;

public sealed class ApplicationRoleListItemDto
{
    public Guid Id { get; init; }

    public Guid EnterpriseApplicationId { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Code { get; init; } = string.Empty;

    public bool IsHighPrivilege { get; init; }

    public int PermissionCount { get; init; }
}