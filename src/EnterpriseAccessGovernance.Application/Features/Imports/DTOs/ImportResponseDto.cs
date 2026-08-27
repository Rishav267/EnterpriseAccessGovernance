namespace EnterpriseAccessGovernance.Application.Features.Imports.DTOs;

public sealed class ImportResponseDto
{
    public Guid ImportBatchId { get; init; }

    public string Status { get; init; } = string.Empty;

    public int TotalRecords { get; init; }

    public int SuccessfulRecords { get; init; }

    public int FailedRecords { get; init; }

    public IReadOnlyCollection<string> Errors { get; init; }
        = Array.Empty<string>();
}