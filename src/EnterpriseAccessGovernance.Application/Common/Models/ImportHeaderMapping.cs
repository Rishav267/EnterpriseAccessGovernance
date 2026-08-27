using EnterpriseAccessGovernance.Application.Common.Models;

namespace EnterpriseAccessGovernance.Application.Common.Models;

public sealed class ImportHeaderMapping
{
    public ImportField Field { get; init; }

    public string SourceHeader { get; init; } = string.Empty;
}