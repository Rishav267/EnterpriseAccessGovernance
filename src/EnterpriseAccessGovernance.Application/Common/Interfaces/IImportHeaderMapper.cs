using EnterpriseAccessGovernance.Application.Common.Models;

namespace EnterpriseAccessGovernance.Application.Common.Interfaces;

public interface IImportHeaderMapper
{
    ImportMappingResult Map(
        IReadOnlyCollection<string> headers);
}