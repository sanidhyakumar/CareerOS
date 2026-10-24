using CareerOS.Application.Features.Tags.Commands.CreateTag;
using CareerOS.Application.Features.Tags.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CareerOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TagsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TagDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllTagsQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TagDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateTagCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteTagCommand(id), cancellationToken);
        return NoContent();
    }
}

