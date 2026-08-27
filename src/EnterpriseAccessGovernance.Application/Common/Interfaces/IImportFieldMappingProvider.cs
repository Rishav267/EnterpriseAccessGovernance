using EnterpriseAccessGovernance.Application.Common.Models;

namespace EnterpriseAccessGovernance.Application.Common.Interfaces;

public interface IImportFieldMappingProvider
{
    IReadOnlyCollection<ImportFieldDefinition> GetDefinitions();
}