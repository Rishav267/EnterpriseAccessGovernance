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
}