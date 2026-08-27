namespace EnterpriseAccessGovernance.Application.Common.Models;

public sealed class ImportProcessingError
{
    public int RowNumber { get; init; }

    public string ErrorMessage { get; init; } = string.Empty;
}