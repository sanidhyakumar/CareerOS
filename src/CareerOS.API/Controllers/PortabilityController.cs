using CareerOS.Application.Features.Portability.DTOs;
using CareerOS.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CareerOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PortabilityController : ControllerBase
{
    private readonly IDataPortabilityService _portabilityService;

    public PortabilityController(IDataPortabilityService portabilityService)
    {
        _portabilityService = portabilityService;
    }

    /// <summary>
    /// Export complete local database snapshot as portable JSON
    /// </summary>
    [HttpGet("export/json")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> ExportJson()
    {
        var bytes = await _portabilityService.ExportFullBackupJsonAsync();
        string filename = $"careeros-backup-{DateTime.UtcNow:yyyyMMdd-HHmmss}.json";
        return File(bytes, "application/json", filename);
    }

    /// <summary>
    /// Export specific entity records as CSV (jobs, applications, companies, contacts)
    /// </summary>
    [HttpGet("export/csv/{entity}")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> ExportCsv(string entity)
    {
        var bytes = await _portabilityService.ExportCsvAsync(entity);
        string filename = $"{entity.ToLowerInvariant()}-{DateTime.UtcNow:yyyyMMdd}.csv";
        return File(bytes, "text/csv; charset=utf-8", filename);
    }

    /// <summary>
    /// Export all personal wiki articles and STAR stories as markdown (.md) archive in a ZIP
    /// </summary>
    [HttpGet("export/markdown")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> ExportMarkdownWiki()
    {
        var bytes = await _portabilityService.ExportMarkdownWikiZipAsync();
        string filename = $"careeros-knowledge-base-{DateTime.UtcNow:yyyyMMdd}.zip";
        return File(bytes, "application/zip", filename);
    }

    /// <summary>
    /// Complete Data Portability Export: JSON database + CSV spreadsheets + Markdown wiki + Documents manifest in a single ZIP
    /// </summary>
    [HttpGet("export/zip")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> ExportFullSystemZip()
    {
        var bytes = await _portabilityService.ExportFullSystemZipAsync();
        string filename = $"careeros-full-export-{DateTime.UtcNow:yyyyMMdd-HHmmss}.zip";
        return File(bytes, "application/zip", filename);
    }

    /// <summary>
    /// Restore or import a CareerOS JSON backup package
    /// </summary>
    [HttpPost("import/json")]
    [ProducesResponseType(typeof(ImportSummaryDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ImportSummaryDto>> ImportJson(IFormFile file, [FromQuery] bool overwrite = false)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { message = "No backup file uploaded." });
        }

        using var stream = file.OpenReadStream();
        var counts = await _portabilityService.ImportBackupJsonAsync(stream, overwrite);

        int total = counts.Values.Sum();
        return Ok(new ImportSummaryDto
        {
            Success = true,
            Message = $"Successfully restored {total} records from backup package.",
            RecordsImported = new Dictionary<string, int>(counts)
        });
    }
}
