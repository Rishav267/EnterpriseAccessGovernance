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
        try
        {
            var assignments =
                await _accessAssignmentService
                    .GetByEmployeeIdAsync(
                        employeeId,
                        cancellationToken);

            return Ok(assignments);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpPatch("{accessAssignmentId:guid}/approve")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Approve(
        Guid employeeId,
        Guid accessAssignmentId,
        [FromBody] CertificationActionRequest? request,
        CancellationToken cancellationToken)
    {
        try
        {
            var reviewerEmployeeId =
                GetReviewerEmployeeId(request);

            await _accessAssignmentService
                .ApproveAsync(
                    employeeId,
                    accessAssignmentId,
                    reviewerEmployeeId,
                    request?.Comment,
                    cancellationToken);

            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpPatch("{accessAssignmentId:guid}/revoke")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Revoke(
        Guid employeeId,
        Guid accessAssignmentId,
        [FromBody] CertificationActionRequest? request,
        CancellationToken cancellationToken)
    {
        try
        {
            var reviewerEmployeeId =
                GetReviewerEmployeeId(request);

            await _accessAssignmentService
                .RevokeAsync(
                    employeeId,
                    accessAssignmentId,
                    reviewerEmployeeId,
                    request?.Comment,
                    cancellationToken);

            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpPatch("{accessAssignmentId:guid}/request-modification")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RequestModification(
        Guid employeeId,
        Guid accessAssignmentId,
        [FromBody] CertificationActionRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var reviewerEmployeeId =
                GetReviewerEmployeeId(request);

            await _accessAssignmentService
                .RequestModificationAsync(
                    employeeId,
                    accessAssignmentId,
                    reviewerEmployeeId,
                    request.Comment ?? string.Empty,
                    cancellationToken);

            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpGet("{accessAssignmentId:guid}/certification-history")]
    [ProducesResponseType(
        typeof(IReadOnlyCollection<CertificationReviewDto>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<
        ActionResult<IReadOnlyCollection<CertificationReviewDto>>>
        GetCertificationHistory(
            Guid employeeId,
            Guid accessAssignmentId,
            CancellationToken cancellationToken)
    {
        try
        {
            var history =
                await _accessAssignmentService
                    .GetCertificationHistoryAsync(
                        employeeId,
                        accessAssignmentId,
                        cancellationToken);

            return Ok(history);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    private Guid GetReviewerEmployeeId(
        CertificationActionRequest? request)
    {
        if (request?.ReviewerEmployeeId is null ||
            request.ReviewerEmployeeId == Guid.Empty)
        {
            throw new ArgumentException(
                "Reviewer employee ID is required.");
        }

        return request.ReviewerEmployeeId.Value;
    }
}

public sealed class CertificationActionRequest
{
    public Guid? ReviewerEmployeeId { get; set; }

    public string? Comment { get; set; }
}