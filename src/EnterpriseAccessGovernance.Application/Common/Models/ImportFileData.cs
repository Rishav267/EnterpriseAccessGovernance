namespace EnterpriseAccessGovernance.Application.Common.Models;

public sealed class ImportFileData
{
    public IReadOnlyList<string> Headers { get; init; } =
        Array.Empty<string>();

    public IReadOnlyList<IReadOnlyDictionary<string, string?>> Rows { get; init; } =
        Array.Empty<IReadOnlyDictionary<string, string?>>();
}