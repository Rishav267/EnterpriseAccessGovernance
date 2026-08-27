using EnterpriseAccessGovernance.Application.Features.Employees;
using EnterpriseAccessGovernance.Application.Features.Employees.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseAccessGovernance.Api.Controllers;

[ApiController]
[Route("api/employees")]
public sealed class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(
        IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyCollection<EmployeeListItemDto>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<EmployeeListItemDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var employees =
            await _employeeService.GetAllAsync(
                cancellationToken);

        return Ok(employees);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(
    typeof(EmployeeListItemDto),
    StatusCodes.Status200OK)]
    [ProducesResponseType(
    StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EmployeeListItemDto>> GetById(
    Guid id,
    CancellationToken cancellationToken)
    {
        var employee =
            await _employeeService.GetByIdAsync(
                id,
                cancellationToken);

        if (employee is null)
        {
            return NotFound();
        }

        return Ok(employee);
    }
}