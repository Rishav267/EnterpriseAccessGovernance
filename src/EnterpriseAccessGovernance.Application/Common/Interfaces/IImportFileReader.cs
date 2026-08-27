using EnterpriseAccessGovernance.Application.Common.Models;

namespace EnterpriseAccessGovernance.Application.Common.Interfaces;

public interface IImportFileReader
{
    bool CanRead(string fileExtension);

    Task<ImportFileData> ReadAsync(
        Stream fileStream,
        CancellationToken cancellationToken = default);
}