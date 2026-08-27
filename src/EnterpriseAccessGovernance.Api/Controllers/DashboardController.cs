using EnterpriseAccessGovernance.Application.Common.Interfaces;
using EnterpriseAccessGovernance.Application.Features.Dashboard.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseAccessGovernance.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
public sealed class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(
        IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("summary")]
    [ProducesResponseType(
        typeof(DashboardSummaryDto),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<DashboardSummaryDto>> GetSummary(
        CancellationToken cancellationToken)
    {
        var summary =
            await _dashboardService.GetSummaryAsync(
                cancellationToken);

        return Ok(summary);
    }
}