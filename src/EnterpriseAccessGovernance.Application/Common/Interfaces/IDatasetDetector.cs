using EnterpriseAccessGovernance.Application.Common.Models;

namespace EnterpriseAccessGovernance.Application.Common.Interfaces;

public interface IDatasetDetector
{
    DatasetDetectionResult Detect(
        IReadOnlyCollection<ImportField> mappedFields);
}