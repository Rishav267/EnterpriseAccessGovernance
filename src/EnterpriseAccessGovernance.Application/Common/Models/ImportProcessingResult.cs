namespace EnterpriseAccessGovernance.Application.Common.Models;

public sealed class ImportProcessingResult
{
    public bool IsSuccess { get; init; }

    public bool IsPartialFailure { get; init; }

    public string? ErrorMessage { get; init; }

    public IReadOnlyCollection<ImportProcessingError> Errors { get; init; }
        = Array.Empty<ImportProcessingError>();

    public static ImportProcessingResult Success()
    {
        return new ImportProcessingResult
        {
            IsSuccess = true,
            IsPartialFailure = false
        };
    }

    public static ImportProcessingResult Success(
        IReadOnlyCollection<ImportProcessingError> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);

        return new ImportProcessingResult
        {
            IsSuccess = true,
            IsPartialFailure = false,
            Errors = errors
        };
    }

    public static ImportProcessingResult Failure(
        string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(errorMessage))
        {
            throw new ArgumentException(
                "Error message is required.",
                nameof(errorMessage));
        }

        return new ImportProcessingResult
        {
            IsSuccess = false,
            IsPartialFailure = false,
            ErrorMessage = errorMessage.Trim()
        };
    }

    public static ImportProcessingResult PartialFailure(
        IReadOnlyCollection<ImportProcessingError> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);

        if (errors.Count == 0)
        {
            throw new ArgumentException(
                "At least one processing error is required.",
                nameof(errors));
        }

        return new ImportProcessingResult
        {
            IsSuccess = false,
            IsPartialFailure = true,
            Errors = errors
        };
    }
}