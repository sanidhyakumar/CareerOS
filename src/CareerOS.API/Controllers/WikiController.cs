using CareerOS.Application.Features.Wiki.Commands.CreateWikiArticle;
using CareerOS.Application.Features.Wiki.Commands.DeleteWikiArticle;
using CareerOS.Application.Features.Wiki.Commands.ToggleWikiStar;
using CareerOS.Application.Features.Wiki.Commands.UpdateWikiArticle;
using CareerOS.Application.Features.Wiki.DTOs;
using CareerOS.Application.Features.Wiki.Queries.GetAllWikiArticles;
using CareerOS.Application.Features.Wiki.Queries.GetWikiArticleById;
using CareerOS.Application.Features.Wiki.Queries.GetWikiArticleBySlug;
using CareerOS.Application.Features.Wiki.Queries.GetWikiStats;
using CareerOS.Application.Features.Wiki.Queries.GetWikiTags;
using CareerOS.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CareerOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WikiController : ControllerBase
{
    private readonly IMediator _mediator;

    public WikiController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<WikiArticleDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? searchTerm,
        [FromQuery] WikiCategory? category,
        [FromQuery] string? tag,
        [FromQuery] Guid? companyId,
        [FromQuery] bool? isStarred,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllWikiArticlesQuery(
            SearchTerm: searchTerm,
            Category: category,
            Tag: tag,
            CompanyId: companyId,
            IsStarred: isStarred), cancellationToken);

        return Ok(result);
    }

    [HttpGet("stats")]
    [ProducesResponseType(typeof(WikiStatsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStats(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetWikiStatsQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("tags")]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTags(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetWikiTagsQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(WikiArticleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, [FromQuery] bool incrementView = true, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetWikiArticleByIdQuery(id, incrementView), cancellationToken);
        return Ok(result);
    }

    [HttpGet("slug/{slug}")]
    [ProducesResponseType(typeof(WikiArticleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBySlug(string slug, [FromQuery] bool incrementView = true, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetWikiArticleBySlugQuery(slug, incrementView), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(WikiArticleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateWikiArticleCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(WikiArticleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateWikiArticleCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            return BadRequest("ID in path does not match ID in body.");
        }

        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/star")]
    [ProducesResponseType(typeof(WikiArticleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleStar(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ToggleWikiStarCommand(id), cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteWikiArticleCommand(id), cancellationToken);
        return NoContent();
    }
}
