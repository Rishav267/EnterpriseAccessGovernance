namespace EnterpriseAccessGovernance.Application.Features.AccessAssignments.DTOs;

public sealed class CertificationRequestDto
{
    public Guid AccessAssignmentId { get; init; }

    public Guid ReviewerEmployeeId { get; init; }

    public string Decision { get; init; } = string.Empty;

    public string? Comment { get; init; }
}