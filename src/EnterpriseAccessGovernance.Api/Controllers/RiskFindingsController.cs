using EnterpriseAccessGovernance.Application.Features.RiskFindings;
using EnterpriseAccessGovernance.Application.Features.RiskFindings.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseAccessGovernance.Api.Controllers;

[ApiController]
[Route("api/employees/{employeeId:guid}/risk-findings")]
public sealed class RiskFindingsController : ControllerBase
{
    private readonly IRiskFindingService _riskFindingService;
    private readonly IRiskDetectionService _riskDetectionService;

    public RiskFindingsController(
        IRiskFindingService riskFindingService, IRiskDetectionService riskDetectionService)
    {
        _riskFindingService = riskFindingService;
        _riskDetectionService = riskDetectionService;
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyCollection<RiskFindingListItemDto>),
        StatusCodes.Status200OK)]
    public async Task<
        ActionResult<IReadOnlyCollection<RiskFindingListItemDto>>>
        GetByEmployeeId(
            Guid employeeId,
            CancellationToken cancellationToken)
    {
        var findings =
            await _riskFindingService
                .GetByEmployeeIdAsync(
                    employeeId,
                    cancellationToken);

        return Ok(findings);
    }

    [HttpPatch("{riskFindingId:guid}/resolve")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Resolve(
        Guid employeeId,
        Guid riskFindingId,
        CancellationToken cancellationToken)
    {
        try
        {
            await _riskFindingService.ResolveAsync(
                employeeId,
                riskFindingId,
                cancellationToken);

            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPatch("{riskFindingId:guid}/ignore")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Ignore(
        Guid employeeId,
        Guid riskFindingId,
        CancellationToken cancellationToken)
    {
        try
        {
            await _riskFindingService.IgnoreAsync(
                employeeId,
                riskFindingId,
                cancellationToken);

            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}