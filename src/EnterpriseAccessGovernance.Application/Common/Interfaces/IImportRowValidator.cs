using EnterpriseAccessGovernance.Application.Common.Models;

namespace EnterpriseAccessGovernance.Application.Common.Interfaces;

public interface IImportRowValidator
{
    ImportRowValidationResult Validate(
        CanonicalImportRow row,
        DatasetType datasetType);
}