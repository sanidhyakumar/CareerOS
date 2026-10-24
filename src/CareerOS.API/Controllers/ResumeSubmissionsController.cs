using CareerOS.Application.Features.ResumeCenter.Commands.Submissions;
using CareerOS.Application.Features.ResumeCenter.DTOs;
using CareerOS.Application.Features.ResumeCenter.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CareerOS.API.Controllers;

[ApiController]
[Route("api/resume-submissions")]
public class ResumeSubmissionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ResumeSubmissionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ResumeSubmissionDto>>> GetAll(
        [FromQuery] Guid? profileId = null,
        [FromQuery] Guid? versionId = null)
    {
        var result = await _mediator.Send(new GetAllResumeSubmissionsQuery(profileId, versionId));
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ResumeSubmissionDto>> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetResumeSubmissionByIdQuery(id));
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ResumeSubmissionDto>> Create([FromBody] CreateResumeSubmissionCommandDto dto)
    {
        var command = new CreateResumeSubmissionCommand(
            dto.ResumeProfileId,
            dto.ResumeVersionId,
            dto.CompanyName,
            dto.RoleTitle,
            dto.SubmissionDate,
            dto.Channel,
            dto.Status,
            dto.CoverLetterId,
            dto.PortfolioVersionId,
            dto.Notes);

        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ResumeSubmissionDto>> Update(Guid id, [FromBody] UpdateResumeSubmissionCommandDto dto)
    {
        var command = new UpdateResumeSubmissionCommand(
            id,
            dto.CompanyName,
            dto.RoleTitle,
            dto.SubmissionDate,
            dto.Channel,
            dto.Status,
            dto.CoverLetterId,
            dto.PortfolioVersionId,
            dto.Notes);

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteResumeSubmissionCommand(id));
        return NoContent();
    }

    [HttpGet("{id:guid}/snapshot")]
    public async Task<ActionResult<SubmissionSnapshotDto>> GetSnapshot(Guid id)
    {
        var result = await _mediator.Send(new GetSubmissionSnapshotQuery(id));
        return Ok(result);
    }
}

