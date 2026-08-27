namespace EnterpriseAccessGovernance.Application.Common.Models;

public sealed class ImportRowValidationResult
{
    public bool IsValid =>
        Errors.Count == 0;

    public IReadOnlyCollection<string> Errors { get; init; }
        = Array.Empty<string>();
}