using EnterpriseAccessGovernance.Application.Common.Interfaces;
using EnterpriseAccessGovernance.Application.Features.RiskFindings.DTOs;

namespace EnterpriseAccessGovernance.Application.Features.RiskFindings;

public sealed class RiskFindingService
    : IRiskFindingService
{
    private readonly IRiskFindingRepository
        _riskFindingRepository;

    public RiskFindingService(
        IRiskFindingRepository riskFindingRepository)
    {
        _riskFindingRepository =
            riskFindingRepository
            ?? throw new ArgumentNullException(
                nameof(riskFindingRepository));
    }

    public Task<
        IReadOnlyCollection<RiskFindingListItemDto>>
        GetByEmployeeIdAsync(
            Guid employeeId,
            CancellationToken cancellationToken = default)
    {
        if (employeeId == Guid.Empty)
        {
            throw new ArgumentException(
                "Employee ID is required.",
                nameof(employeeId));
        }

        return _riskFindingRepository
            .GetByEmployeeIdAsync(
                employeeId,
                cancellationToken);
    }

    public async Task ResolveAsync(
        Guid employeeId,
        Guid riskFindingId,
        CancellationToken cancellationToken = default)
    {
        var riskFinding =
            await GetAndValidateAsync(
                employeeId,
                riskFindingId,
                cancellationToken);

        riskFinding.Resolve(
            DateTime.UtcNow);

        await _riskFindingRepository
            .SaveChangesAsync(
                cancellationToken);
    }

    public async Task IgnoreAsync(
        Guid employeeId,
        Guid riskFindingId,
        CancellationToken cancellationToken = default)
    {
        var riskFinding =
            await GetAndValidateAsync(
                employeeId,
                riskFindingId,
                cancellationToken);

        riskFinding.Ignore();

        await _riskFindingRepository
            .SaveChangesAsync(
                cancellationToken);
    }

    private async Task<Domain.Entities.RiskFinding>
        GetAndValidateAsync(
            Guid employeeId,
            Guid riskFindingId,
            CancellationToken cancellationToken)
    {
        if (employeeId == Guid.Empty)
        {
            throw new ArgumentException(
                "Employee ID is required.",
                nameof(employeeId));
        }

        if (riskFindingId == Guid.Empty)
        {
            throw new ArgumentException(
                "Risk finding ID is required.",
                nameof(riskFindingId));
        }

        var riskFinding =
            await _riskFindingRepository.GetByIdAsync(
                riskFindingId,
                cancellationToken);

        if (riskFinding is null ||
            riskFinding.EmployeeId != employeeId)
        {
            throw new KeyNotFoundException(
                "Risk finding was not found for this employee.");
        }

        return riskFinding;
    }

    public async Task<PagedRiskFindingResultDto>
    GetPagedAsync(
        RiskFindingQueryDto query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        if (query.PageNumber <= 0)
        {
            throw new ArgumentException(
                "Page number must be greater than zero.",
                nameof(query.PageNumber));
        }

        if (query.PageSize <= 0 ||
            query.PageSize > 100)
        {
            throw new ArgumentException(
                "Page size must be between 1 and 100.",
                nameof(query.PageSize));
        }

        var result =
            await _riskFindingRepository
                .GetPagedAsync(
                    query,
                    cancellationToken);

        return new PagedRiskFindingResultDto
        {
            Items = result.Items,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            TotalCount = result.TotalCount
        };
    }

    public Task<RiskFindingSummaryDto>
    GetSummaryAsync(
        CancellationToken cancellationToken = default)
    {
        return _riskFindingRepository
            .GetSummaryAsync(
                cancellationToken);
    }
}