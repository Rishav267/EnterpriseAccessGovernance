using EnterpriseAccessGovernance.Domain.Common;

namespace EnterpriseAccessGovernance.Domain.Entities;

public sealed class ImportError : BaseEntity
{
    private ImportError()
    {
    }

    private ImportError(
        Guid importBatchId,
        int rowNumber,
        string errorMessage)
    {
        ImportBatchId = importBatchId;
        RowNumber = rowNumber;
        ErrorMessage = errorMessage;
    }

    public Guid ImportBatchId { get; private set; }

    public int RowNumber { get; private set; }

    public string ErrorMessage { get; private set; } = string.Empty;

    public ImportBatch? ImportBatch { get; private set; }

    public static ImportError Create(
        Guid importBatchId,
        int rowNumber,
        string errorMessage)
    {
        if (importBatchId == Guid.Empty)
        {
            throw new ArgumentException(
                "Import batch is required.",
                nameof(importBatchId));
        }

        if (rowNumber <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(rowNumber));
        }

        if (string.IsNullOrWhiteSpace(errorMessage))
        {
            throw new ArgumentException(
                "Error message is required.",
                nameof(errorMessage));
        }

        return new ImportError(
            importBatchId,
            rowNumber,
            errorMessage.Trim());
    }
}