using EnterpriseAccessGovernance.Application.Features.Applications;
using EnterpriseAccessGovernance.Application.Features.Applications.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseAccessGovernance.Api.Controllers;

[ApiController]
[Route("api/applications")]
public sealed class ApplicationsController : ControllerBase
{
    private readonly IApplicationService _applicationService;

    public ApplicationsController(
        IApplicationService applicationService)
    {
        _applicationService = applicationService;
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyCollection<ApplicationListItemDto>),
        StatusCodes.Status200OK)]
    public async Task<
        ActionResult<IReadOnlyCollection<ApplicationListItemDto>>>
        GetAll(
            CancellationToken cancellationToken)
    {
        var applications =
            await _applicationService.GetAllAsync(
                cancellationToken);

        return Ok(applications);
    }

    [HttpGet("{applicationId:guid}")]
    [ProducesResponseType(
    typeof(ApplicationDetailsDto),
    StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApplicationDetailsDto>> GetById(
    Guid applicationId,
    CancellationToken cancellationToken)
    {
        var application =
            await _applicationService.GetByIdAsync(
                applicationId,
                cancellationToken);

        if (application is null)
        {
            return NotFound();
        }

        return Ok(application);
    }
}