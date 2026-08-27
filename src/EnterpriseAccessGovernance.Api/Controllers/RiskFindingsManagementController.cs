using EnterpriseAccessGovernance.Application.Features.RiskFindings;
using EnterpriseAccessGovernance.Application.Features.RiskFindings.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseAccessGovernance.Api.Controllers;

[ApiController]
[Route("api/risk-findings")]
public sealed class RiskFindingsManagementController
    : ControllerBase
{
    private readonly IRiskFindingService _riskFindingService;

    public RiskFindingsManagementController(
        IRiskFindingService riskFindingService)
    {
        _riskFindingService =
            riskFindingService
            ?? throw new ArgumentNullException(
                nameof(riskFindingService));
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(PagedRiskFindingResultDto),
        StatusCodes.Status200OK)]
    public async Task<
        ActionResult<PagedRiskFindingResultDto>>
        Get(
            [FromQuery] RiskFindingQueryDto query,
            CancellationToken cancellationToken)
    {
        try
        {
            var result =
                await _riskFindingService
                    .GetPagedAsync(
                        query,
                        cancellationToken);

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpGet("summary")]
    [ProducesResponseType(
        typeof(RiskFindingSummaryDto),
        StatusCodes.Status200OK)]
    public async Task<
        ActionResult<RiskFindingSummaryDto>>
        GetSummary(
            CancellationToken cancellationToken)
    {
        var summary =
            await _riskFindingService
                .GetSummaryAsync(
                    cancellationToken);

        return Ok(summary);
    }
}