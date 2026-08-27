namespace EnterpriseAccessGovernance.Application.Features.Applications.DTOs;

public sealed class ApplicationDetailsDto
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Code { get; init; } = string.Empty;

    public string? Description { get; init; }

    public int RoleCount { get; init; }
}