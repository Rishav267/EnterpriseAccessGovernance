using EnterpriseAccessGovernance.Application.Features.Reports;
using EnterpriseAccessGovernance.Application.Features.Reports.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseAccessGovernance.Api.Controllers;

[ApiController]
[Route("api/reports")]
public sealed class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("high-risk-users")]
    [ProducesResponseType(
        typeof(IReadOnlyCollection<HighRiskUserDto>),
        StatusCodes.Status200OK)]
    public async Task<
        ActionResult<IReadOnlyCollection<HighRiskUserDto>>>
        GetHighRiskUsers(
            CancellationToken cancellationToken)
    {
        var result =
            await _reportService.GetHighRiskUsersAsync(
                cancellationToken);

        return Ok(result);
    }

    [HttpGet("dormant-accounts")]
    [ProducesResponseType(
        typeof(IReadOnlyCollection<DormantAccountDto>),
        StatusCodes.Status200OK)]
    public async Task<
        ActionResult<IReadOnlyCollection<DormantAccountDto>>>
        GetDormantAccounts(
            [FromQuery] int dormantDays = 90,
            CancellationToken cancellationToken = default)
    {
        if (dormantDays <= 0)
        {
            return BadRequest(
                "dormantDays must be greater than zero.");
        }

        var result =
            await _reportService.GetDormantAccountsAsync(
                dormantDays,
                cancellationToken);

        return Ok(result);
    }

    [HttpGet("access-by-department")]
    [ProducesResponseType(
        typeof(IReadOnlyCollection<AccessByDepartmentDto>),
        StatusCodes.Status200OK)]
    public async Task<
        ActionResult<IReadOnlyCollection<AccessByDepartmentDto>>>
        GetAccessByDepartment(
            CancellationToken cancellationToken)
    {
        var result =
            await _reportService.GetAccessByDepartmentAsync(
                cancellationToken);

        return Ok(result);
    }

    [HttpGet("access-by-application")]
    [ProducesResponseType(
        typeof(IReadOnlyCollection<AccessByApplicationDto>),
        StatusCodes.Status200OK)]
    public async Task<
        ActionResult<IReadOnlyCollection<AccessByApplicationDto>>>
        GetAccessByApplication(
            CancellationToken cancellationToken)
    {
        var result =
            await _reportService.GetAccessByApplicationAsync(
                cancellationToken);

        return Ok(result);
    }

    [HttpGet("pending-certifications")]
    [ProducesResponseType(
        typeof(IReadOnlyCollection<PendingCertificationDto>),
        StatusCodes.Status200OK)]
    public async Task<
        ActionResult<IReadOnlyCollection<PendingCertificationDto>>>
        GetPendingCertifications(
            CancellationToken cancellationToken)
    {
        var result =
            await _reportService.GetPendingCertificationsAsync(
                cancellationToken);

        return Ok(result);
    }

    [HttpGet("certification-summary")]
    [ProducesResponseType(
        typeof(CertificationSummaryDto),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<CertificationSummaryDto>>
        GetCertificationSummary(
            CancellationToken cancellationToken)
    {
        var result =
            await _reportService.GetCertificationSummaryAsync(
                cancellationToken);

        return Ok(result);
    }
}