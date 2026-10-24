using CareerOS.Application.Features.Search.Commands.ReindexAll;
using CareerOS.Application.Features.Search.DTOs;
using CareerOS.Application.Features.Search.Queries.GetSearchStats;
using CareerOS.Application.Features.Search.Queries.GlobalSearch;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CareerOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly IMediator _mediator;

    public SearchController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Perform unified full-text search across all career entities with BM25 ranking and snippet generation
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(SearchSummaryDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<SearchSummaryDto>> Search(
        [FromQuery] string q,
        [FromQuery] string? type = null,
        [FromQuery] int limit = 30)
    {
        var result = await _mediator.Send(new GlobalSearchQuery(q, type, limit));
        return Ok(result);
    }

    /// <summary>
    /// Retrieve search engine statistics breakdown by entity type
    /// </summary>
    [HttpGet("stats")]
    [ProducesResponseType(typeof(SearchStatsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<SearchStatsDto>> GetStats()
    {
        var stats = await _mediator.Send(new GetSearchStatsQuery());
        return Ok(stats);
    }

    /// <summary>
    /// Trigger an on-demand re-index of all SQLite FTS5 records across the system
    /// </summary>
    [HttpPost("reindex")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public async Task<ActionResult<int>> Reindex()
    {
        int count = await _mediator.Send(new ReindexAllCommand());
        return Ok(new { totalIndexed = count, message = $"Indexed {count} records into SQLite FTS5" });
    }
}

