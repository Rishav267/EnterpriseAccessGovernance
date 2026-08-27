using EnterpriseAccessGovernance.Application.Common.Interfaces;
using EnterpriseAccessGovernance.Application.Features.AccessAssignments.DTOs;

namespace EnterpriseAccessGovernance.Application.Features.AccessAssignments;

public sealed class AccessAssignmentService
    : IAccessAssignmentService
{
    private readonly IAccessAssignmentRepository
        _accessAssignmentRepository;

    public AccessAssignmentService(
        IAccessAssignmentRepository accessAssignmentRepository)
    {
        _accessAssignmentRepository =
            accessAssignmentRepository
            ?? throw new ArgumentNullException(
                nameof(accessAssignmentRepository));
    }

    public Task<IReadOnlyCollection<AccessAssignmentListItemDto>>
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

        return _accessAssignmentRepository
            .GetByEmployeeIdAsync(
                employeeId,
                cancellationToken);
    }

    public async Task ApproveAsync(
    Guid employeeId,
    Guid accessAssignmentId,
    CancellationToken cancellationToken = default)
    {
        if (employeeId == Guid.Empty)
        {
            throw new ArgumentException(
                "Employee ID is required.",
                nameof(employeeId));
        }

        if (accessAssignmentId == Guid.Empty)
        {
            throw new ArgumentException(
                "Access assignment ID is required.",
                nameof(accessAssignmentId));
        }

        var assignment =
            await _accessAssignmentRepository.GetByIdAsync(
                accessAssignmentId,
                cancellationToken);

        if (assignment is null ||
            assignment.EmployeeId != employeeId)
        {
            throw new KeyNotFoundException(
                "Access assignment was not found for this employee.");
        }

        assignment.ApproveCertification(
            DateTime.UtcNow);

        await _accessAssignmentRepository.SaveChangesAsync(
            cancellationToken);
    }

    public async Task RevokeAsync(
    Guid employeeId,
    Guid accessAssignmentId,
    CancellationToken cancellationToken = default)
    {
        if (employeeId == Guid.Empty)
        {
            throw new ArgumentException(
                "Employee ID is required.",
                nameof(employeeId));
        }

        if (accessAssignmentId == Guid.Empty)
        {
            throw new ArgumentException(
                "Access assignment ID is required.",
                nameof(accessAssignmentId));
        }

        var assignment =
            await _accessAssignmentRepository.GetByIdAsync(
                accessAssignmentId,
                cancellationToken);

        if (assignment is null ||
            assignment.EmployeeId != employeeId)
        {
            throw new KeyNotFoundException(
                "Access assignment was not found for this employee.");
        }

        assignment.Revoke(
            DateTime.UtcNow);

        await _accessAssignmentRepository.SaveChangesAsync(
            cancellationToken);
    }
}