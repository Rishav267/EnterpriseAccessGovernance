namespace EnterpriseAccessGovernance.Application.Common.Models;

public sealed class DatasetDetectionResult
{
    public DatasetType DatasetType { get; init; }

    public bool IsDetected { get; init; }

    public bool IsAmbiguous { get; init; }

    public IReadOnlyCollection<DatasetType> Candidates { get; init; }
        = Array.Empty<DatasetType>();

    public string? ErrorMessage { get; init; }
}