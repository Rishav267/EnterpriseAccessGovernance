namespace EnterpriseAccessGovernance.Application.Features.ApplicationRoles.DTOs;

public sealed class PermissionListItemDto
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Code { get; init; } = string.Empty;

    public string? Description { get; init; }
}