namespace EnterpriseAccessGovernance.Application.Common.Models;

public sealed class ImportFieldDefinition
{
    public ImportField Field { get; init; }

    public bool Required { get; init; }

    public IReadOnlyCollection<string> Aliases { get; init; } =
        Array.Empty<string>();
}