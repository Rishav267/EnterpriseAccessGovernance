using EnterpriseAccessGovernance.Application.Common.Interfaces;
using EnterpriseAccessGovernance.Application.Common.Models;

namespace EnterpriseAccessGovernance.Infrastructure.Import.Mapping;

public sealed class ImportRowMapper : IImportRowMapper
{
    public CanonicalImportRow Map(
        IReadOnlyDictionary<string, string?> sourceRow,
        IReadOnlyCollection<ImportHeaderMapping> mappings)
    {
        ArgumentNullException.ThrowIfNull(sourceRow);
        ArgumentNullException.ThrowIfNull(mappings);

        var canonicalRow = new CanonicalImportRow();

        foreach (var mapping in mappings)
        {
            if (!sourceRow.TryGetValue(
                    mapping.SourceHeader,
                    out var value))
            {
                continue;
            }

            canonicalRow.Set(
                mapping.Field,
                value);
        }

        return canonicalRow;
    }
}