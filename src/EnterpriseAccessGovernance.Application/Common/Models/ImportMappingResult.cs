namespace EnterpriseAccessGovernance.Application.Common.Models;

public sealed class ImportMappingResult
{
    public IReadOnlyCollection<ImportHeaderMapping> Mappings { get; init; }
        = Array.Empty<ImportHeaderMapping>();

    public IReadOnlyCollection<string> MissingRequiredFields { get; init; }
        = Array.Empty<string>();

    public IReadOnlyCollection<string> UnknownHeaders { get; init; }
        = Array.Empty<string>();

    public IReadOnlyCollection<string> AmbiguousHeaders { get; init; }
        = Array.Empty<string>();

    public bool IsValid =>
        MissingRequiredFields.Count == 0 &&
        AmbiguousHeaders.Count == 0;
}