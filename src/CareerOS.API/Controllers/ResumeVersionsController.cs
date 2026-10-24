using CareerOS.Application.Features.ResumeCenter.Commands.Versions;
using CareerOS.Application.Features.ResumeCenter.DTOs;
using CareerOS.Application.Features.ResumeCenter.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CareerOS.API.Controllers;

[ApiController]
[Route("api/resume-versions")]
public class ResumeVersionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ResumeVersionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ResumeVersionDto>> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetResumeVersionByIdQuery(id));
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ResumeVersionDto>> Update(Guid id, [FromBody] UpdateResumeVersionCommandDto dto)
    {
        var command = new UpdateResumeVersionCommand(
            id,
            dto.VersionName,
            dto.DocumentId,
            dto.FileFormat,
            dto.TargetCompany,
            dto.TargetJobTitle,
            dto.TailoredKeywords,
            dto.Content,
            dto.ChangeNotes,
            dto.IsBaseVersion,
            dto.IsArchived);

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteResumeVersionCommand(id));
        return NoContent();
    }

    [HttpPost("{id:guid}/clone")]
    public async Task<ActionResult<ResumeVersionDto>> Clone(Guid id, [FromQuery] string? newVersionName = null, [FromQuery] string? targetCompany = null)
    {
        var result = await _mediator.Send(new CloneResumeVersionCommand(id, newVersionName, targetCompany));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
}

