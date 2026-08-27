using EnterpriseAccessGovernance.Application.Features.AccessAssignments;
using EnterpriseAccessGovernance.Application.Features.AccessAssignments.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseAccessGovernance.Api.Controllers;

[ApiController]
[Route("api/employees/{employeeId:guid}/access-assignments")]
public sealed class AccessAssignmentsController : ControllerBase
{
    private readonly IAccessAssignmentService
        _accessAssignmentService;

    public AccessAssignmentsController(
        IAccessAssignmentService accessAssignmentService)
    {
        _accessAssignmentService =
            accessAssignmentService;
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyCollection<AccessAssignmentListItemDto>),
        StatusCodes.Status200OK)]
    public async Task<
        ActionResult<IReadOnlyCollection<AccessAssignmentListItemDto>>>
        GetByEmployeeId(
            Guid employeeId,
            CancellationToken cancellationToken)
    {
        var assignments =
            await _accessAssignmentService
                .GetByEmployeeIdAsync(
                    employeeId,
                    cancellationToken);

        return Ok(assignments);
    }

    [HttpPatch("{accessAssignmentId:guid}/approve")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Approve(
    Guid employeeId,
    Guid accessAssignmentId,
    CancellationToken cancellationToken)
    {
        try
        {
            await _accessAssignmentService.ApproveAsync(
                employeeId,
                accessAssignmentId,
                cancellationToken);

            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPatch("{accessAssignmentId:guid}/revoke")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Revoke(
    Guid employeeId,
    Guid accessAssignmentId,
    CancellationToken cancellationToken)
    {
        try
        {
            await _accessAssignmentService.RevokeAsync(
                employeeId,
                accessAssignmentId,
                cancellationToken);

            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}