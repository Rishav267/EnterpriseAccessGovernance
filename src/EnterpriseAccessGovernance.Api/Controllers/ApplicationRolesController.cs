using EnterpriseAccessGovernance.Application.Features.ApplicationRoles;
using EnterpriseAccessGovernance.Application.Features.ApplicationRoles.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseAccessGovernance.Api.Controllers;

[ApiController]
[Route("api/applications/{applicationId:guid}/roles")]
public sealed class ApplicationRolesController : ControllerBase
{
    private readonly IApplicationRoleService
        _applicationRoleService;

    public ApplicationRolesController(
        IApplicationRoleService applicationRoleService)
    {
        _applicationRoleService =
            applicationRoleService;
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyCollection<ApplicationRoleListItemDto>),
        StatusCodes.Status200OK)]
    public async Task<
        ActionResult<IReadOnlyCollection<ApplicationRoleListItemDto>>>
        GetByApplicationId(
            Guid applicationId,
            CancellationToken cancellationToken)
    {
        var roles =
            await _applicationRoleService
                .GetByApplicationIdAsync(
                    applicationId,
                    cancellationToken);

        return Ok(roles);
    }

    [HttpGet("{roleId:guid}/permissions")]
    [ProducesResponseType(
    typeof(IReadOnlyCollection<PermissionListItemDto>),
    StatusCodes.Status200OK)]
    public async Task<
    ActionResult<IReadOnlyCollection<PermissionListItemDto>>>
    GetPermissions(
        Guid applicationId,
        Guid roleId,
        CancellationToken cancellationToken)
    {
        var permissions =
            await _applicationRoleService
                .GetPermissionsAsync(
                    applicationId,
                    roleId,
                    cancellationToken);

        return Ok(permissions);
    }
}