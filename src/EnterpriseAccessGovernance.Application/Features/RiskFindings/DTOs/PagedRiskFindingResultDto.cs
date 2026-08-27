namespace EnterpriseAccessGovernance.Application.Features.RiskFindings.DTOs;

public sealed class PagedRiskFindingResultDto
{
    public IReadOnlyCollection<RiskFindingListItemDto> Items { get; init; }
        = Array.Empty<RiskFindingListItemDto>();

    public int PageNumber { get; init; }

    public int PageSize { get; init; }

    public int TotalCount { get; init; }

    public int TotalPages =>
        PageSize <= 0
            ? 0
            : (int)Math.Ceiling(
                TotalCount / (double)PageSize);
}