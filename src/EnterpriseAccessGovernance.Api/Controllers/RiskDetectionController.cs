using EnterpriseAccessGovernance.Application.Features.RiskFindings;
using EnterpriseAccessGovernance.Application.Features.RiskFindings.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseAccessGovernance.Api.Controllers;

[ApiController]
[Route("api/risk-detection")]
public sealed class RiskDetectionController
    : ControllerBase
{
    private readonly IRiskDetectionService
        _riskDetectionService;

    public RiskDetectionController(
        IRiskDetectionService riskDetectionService)
    {
        _riskDetectionService =
            riskDetectionService;
    }

    [HttpPost]
    [ProducesResponseType(
        typeof(RiskDetectionResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RiskDetectionResponse>>
        Run(
            [FromQuery] int dormantDays = 90,
            [FromQuery] int excessiveApplicationThreshold = 5,
            CancellationToken cancellationToken = default)
    {
        try
        {
            var findingsCreated =
                await _riskDetectionService.RunAsync(
                    dormantDays,
                    excessiveApplicationThreshold,
                    cancellationToken);

            return Ok(
                new RiskDetectionResponse
                {
                    FindingsCreated =
                        findingsCreated
                });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(
                new
                {
                    message = ex.Message
                });
        }
    }
}