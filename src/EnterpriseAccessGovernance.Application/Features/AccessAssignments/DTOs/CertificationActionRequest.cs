namespace EnterpriseAccessGovernance.Application.Features.AccessAssignments.DTOs;

public sealed class CertificationActionRequest
{
    public Guid ReviewerEmployeeId { get; set; }

    public string? Comment { get; set; }
}