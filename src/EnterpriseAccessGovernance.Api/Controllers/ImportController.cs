using EnterpriseAccessGovernance.Application.Features.Imports.DTOs;
using EnterpriseAccessGovernance.Application.Features.Imports.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseAccessGovernance.Api.Controllers;

[ApiController]
[Route("api/import")]
public sealed class ImportController : ControllerBase
{
    private readonly IImportService _importService;

    public ImportController(
        IImportService importService)
    {
        _importService = importService;
    }

    /// <summary>
    /// Imports enterprise access governance data from a CSV or XLSX file.
    /// </summary>
    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(
        typeof(ImportResponseDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ImportResponseDto>> Import(
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file is null)
        {
            return BadRequest(
                "A file is required.");
        }

        if (file.Length == 0)
        {
            return BadRequest(
                "The uploaded file is empty.");
        }

        await using var stream = file.OpenReadStream();

        var result =
            await _importService.ImportAsync(
                stream,
                file.FileName,
                cancellationToken);

        return Ok(result);
    }
}