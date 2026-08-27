using EnterpriseAccessGovernance.Application.Common.Models;

namespace EnterpriseAccessGovernance.Application.Common.Interfaces;

public interface IImportDatasetProcessor
{
    DatasetType DatasetType { get; }

    Task<ImportProcessingResult> ProcessAsync(
        IReadOnlyCollection<CanonicalImportRow> rows,
        CancellationToken cancellationToken = default);
}