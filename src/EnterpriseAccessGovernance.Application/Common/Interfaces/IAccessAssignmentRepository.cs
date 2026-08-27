using EnterpriseAccessGovernance.Application.Features.AccessAssignments.DTOs;
using EnterpriseAccessGovernance.Domain.Entities;

namespace EnterpriseAccessGovernance.Application.Common.Interfaces;

public interface IAccessAssignmentRepository
{
    Task<IReadOnlyCollection<AccessAssignmentListItemDto>>
        GetByEmployeeIdAsync(
            Guid employeeId,
            CancellationToken cancellationToken = default);
    Task<AccessAssignment?> GetByIdAsync(
    Guid accessAssignmentId,
    CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}