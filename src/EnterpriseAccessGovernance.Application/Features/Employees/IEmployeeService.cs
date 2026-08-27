using EnterpriseAccessGovernance.Application.Common.Models;
using EnterpriseAccessGovernance.Application.Features.Employees.DTOs;

namespace EnterpriseAccessGovernance.Application.Features.Employees;

public interface IEmployeeService
{
    Task<IReadOnlyCollection<EmployeeListItemDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<EmployeeListItemDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<PagedResultDto<EmployeeListItemDto>> SearchAsync(
    EmployeeSearchRequest request,
    CancellationToken cancellationToken = default);
}