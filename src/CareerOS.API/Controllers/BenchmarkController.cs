using CareerOS.Application.Features.Benchmark.DTOs;
using CareerOS.Domain.Interfaces;
using CareerOS.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace CareerOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BenchmarkController : ControllerBase
{
    private readonly IBenchmarkService _benchmarkService;

    public BenchmarkController(IBenchmarkService benchmarkService)
    {
        _benchmarkService = benchmarkService;
    }

    /// <summary>
    /// Retrieve real-time system diagnostics, memory footprint, and database health metrics
    /// </summary>
    [HttpGet("health")]
    [ProducesResponseType(typeof(IDictionary<string, object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHealth()
    {
        var diagnostics = await _benchmarkService.GetSystemDiagnosticsAsync();
        return Ok(diagnostics);
    }

    /// <summary>
    /// Execute scale benchmark validating 10k Applications, 50k Documents metadata, and 100k SQLite FTS5 search records
    /// </summary>
    [HttpPost("run")]
    [ProducesResponseType(typeof(BenchmarkSummary), StatusCodes.Status200OK)]
    public async Task<ActionResult<BenchmarkSummary>> RunBenchmark([FromBody] RunBenchmarkRequest? request)
    {
        var req = request ?? new RunBenchmarkRequest();
        var summary = await _benchmarkService.ExecuteScaleBenchmarkAsync(
            req.ApplicationCount,
            req.DocumentCount,
            req.SearchRecordCount,
            req.CleanupAfterBenchmark
        );
        return Ok(summary);
    }
}

