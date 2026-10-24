using CareerOS.Application.Features.ResumeCenter.Commands.Profiles;
using CareerOS.Application.Features.ResumeCenter.Commands.Versions;
using CareerOS.Application.Features.ResumeCenter.DTOs;
using CareerOS.Application.Features.ResumeCenter.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CareerOS.API.Controllers;

[ApiController]
[Route("api/resume-profiles")]
public class ResumeProfilesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ResumeProfilesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ResumeProfileDto>>> GetAll([FromQuery] bool includeArchived = false)
    {
        var result = await _mediator.Send(new GetAllResumeProfilesQuery(includeArchived));
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ResumeProfileDto>> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetResumeProfileByIdQuery(id));
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ResumeProfileDto>> Create([FromBody] CreateResumeProfileCommandDto dto)
    {
        var command = new CreateResumeProfileCommand(
            dto.Name,
            dto.TargetRole,
            dto.TargetIndustry,
            dto.Summary,
            dto.PrimarySkills,
            dto.SecondarySkills,
            dto.IsDefault);

        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ResumeProfileDto>> Update(Guid id, [FromBody] UpdateResumeProfileCommandDto dto)
    {
        var command = new UpdateResumeProfileCommand(
            id,
            dto.Name,
            dto.TargetRole,
            dto.TargetIndustry,
            dto.Summary,
            dto.PrimarySkills,
            dto.SecondarySkills,
            dto.IsDefault,
            dto.IsArchived);

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteResumeProfileCommand(id));
        return NoContent();
    }

    [HttpPost("{id:guid}/set-default")]
    public async Task<IActionResult> SetDefault(Guid id)
    {
        await _mediator.Send(new SetDefaultResumeProfileCommand(id));
        return NoContent();
    }

    [HttpGet("{id:guid}/versions")]
    public async Task<ActionResult<IReadOnlyList<ResumeVersionDto>>> GetVersions(Guid id, [FromQuery] bool includeArchived = false)
    {
        var result = await _mediator.Send(new GetResumeVersionsByProfileQuery(id, includeArchived));
        return Ok(result);
    }

    [HttpPost("{id:guid}/versions")]
    public async Task<ActionResult<ResumeVersionDto>> CreateVersion(Guid id, [FromBody] CreateResumeVersionCommandDto dto)
    {
        var command = new CreateResumeVersionCommand(
            id,
            dto.VersionName,
            dto.DocumentId,
            dto.FileFormat,
            dto.TargetCompany,
            dto.TargetJobTitle,
            dto.TailoredKeywords,
            dto.Content,
            dto.ChangeNotes,
            dto.IsBaseVersion);

        var result = await _mediator.Send(command);
        return Created($"/api/resume-versions/{result.Id}", result);
    }
}

