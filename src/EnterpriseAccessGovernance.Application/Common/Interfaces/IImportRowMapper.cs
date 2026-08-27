using EnterpriseAccessGovernance.Application.Common.Models;

namespace EnterpriseAccessGovernance.Application.Common.Interfaces;

public interface IImportRowMapper
{
    CanonicalImportRow Map(
        IReadOnlyDictionary<string, string?> sourceRow,
        IReadOnlyCollection<ImportHeaderMapping> mappings);
}