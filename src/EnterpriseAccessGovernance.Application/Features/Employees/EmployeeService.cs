using EnterpriseAccessGovernance.Application.Common.Interfaces;
using EnterpriseAccessGovernance.Application.Common.Models;
using EnterpriseAccessGovernance.Application.Features.Employees.DTOs;

namespace EnterpriseAccessGovernance.Application.Features.Employees;

public sealed class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;

    public EmployeeService(
        IEmployeeRepository employeeRepository)
    {
        _employeeRepository =
            employeeRepository
            ?? throw new ArgumentNullException(
                nameof(employeeRepository));
    }

    public Task<IReadOnlyCollection<EmployeeListItemDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return _employeeRepository.GetAllAsync(
            cancellationToken);
    }

    public Task<EmployeeListItemDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _employeeRepository.GetByIdAsync(
            id,
            cancellationToken);
    }

    public Task<PagedResultDto<EmployeeListItemDto>> SearchAsync(
    EmployeeSearchRequest request,
    CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        return _employeeRepository.SearchAsync(
            request,
            cancellationToken);
    }
}