using EnterpriseAccessGovernance.Domain.Common;
using EnterpriseAccessGovernance.Domain.Enums;

namespace EnterpriseAccessGovernance.Domain.Entities;

public sealed class ImportBatch : AuditableEntity
{
    private readonly List<ImportError> _errors = [];

    private ImportBatch()
    {
    }

    private ImportBatch(
        string fileName,
        string fileType,
        DateTime startedAtUtc)
    {
        FileName = fileName;
        FileType = fileType;
        StartedAtUtc = startedAtUtc;
        Status = ImportStatus.Pending;
    }

    public string FileName { get; private set; } = string.Empty;

    public string FileType { get; private set; } = string.Empty;

    public ImportStatus Status { get; private set; }

    public int TotalRecords { get; private set; }

    public int SuccessfullyProcessedRecords { get; private set; }

    public int FailedRecords { get; private set; }

    public DateTime StartedAtUtc { get; private set; }

    public DateTime? CompletedAtUtc { get; private set; }

    public IReadOnlyCollection<ImportError> Errors =>
        _errors.AsReadOnly();

    public static ImportBatch Create(
        string fileName,
        string fileType,
        DateTime startedAtUtc)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException(
                "File name is required.",
                nameof(fileName));
        }

        if (string.IsNullOrWhiteSpace(fileType))
        {
            throw new ArgumentException(
                "File type is required.",
                nameof(fileType));
        }

        return new ImportBatch(
            fileName.Trim(),
            fileType.Trim().ToUpperInvariant(),
            startedAtUtc);
    }

    public void StartProcessing(int totalRecords)
    {
        if (totalRecords < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(totalRecords));
        }

        TotalRecords = totalRecords;
        Status = ImportStatus.Processing;

        MarkUpdated();
    }

    public void RecordSuccess()
    {
        if (Status != ImportStatus.Processing)
        {
            throw new InvalidOperationException(
                "Import batch is not currently processing.");
        }

        SuccessfullyProcessedRecords++;

        MarkUpdated();
    }

    public void RecordFailure()
    {
        if (Status != ImportStatus.Processing)
        {
            throw new InvalidOperationException(
                "Import batch is not currently processing.");
        }

        FailedRecords++;

        MarkUpdated();
    }

    public void AddError(ImportError error)
    {
        ArgumentNullException.ThrowIfNull(error);

        _errors.Add(error);

        MarkUpdated();
    }

    public void Complete(DateTime completedAtUtc)
    {
        if (Status != ImportStatus.Processing)
        {
            throw new InvalidOperationException(
                "Only a processing import batch can be completed.");
        }

        CompletedAtUtc = completedAtUtc;

        Status = FailedRecords > 0
            ? ImportStatus.CompletedWithErrors
            : ImportStatus.Completed;

        MarkUpdated();
    }

    public void Fail(DateTime failedAtUtc)
    {
        Status = ImportStatus.Failed;
        CompletedAtUtc = failedAtUtc;

        MarkUpdated();
    }
}