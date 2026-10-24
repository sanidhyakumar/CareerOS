using CareerOS.Application.Features.Resumes.Commands.CreateResume;
using CareerOS.Application.Features.Resumes.Commands.DeleteResume;
using CareerOS.Application.Features.Resumes.Commands.UpdateResume;
using CareerOS.Application.Features.Resumes.DTOs;
using CareerOS.Application.Features.Resumes.Queries.GetAllResumes;
using CareerOS.Application.Features.Resumes.Queries.GetResumeById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CareerOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ResumesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ResumesController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ResumeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new GetAllResumesQuery(), cancellationToken));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ResumeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new GetResumeByIdQuery(id), cancellationToken));

    [HttpPost]
    [ProducesResponseType(typeof(ResumeDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateResumeCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ResumeDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateResumeRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateResumeCommand(id, request.Name, request.Content, request.IsDefault, request.Tags);
        return Ok(await _mediator.Send(command, cancellationToken));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteResumeCommand(id), cancellationToken);
        return NoContent();
    }
}

public record UpdateResumeRequest(string Name, string? Content, bool IsDefault, string? Tags);

