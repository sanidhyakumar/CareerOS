using CareerOS.Application.Features.ResumeCenter.Commands.CoverLetters;
using CareerOS.Application.Features.ResumeCenter.DTOs;
using CareerOS.Application.Features.ResumeCenter.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CareerOS.API.Controllers;

[ApiController]
[Route("api/cover-letters")]
public class CoverLettersController : ControllerBase
{
    private readonly IMediator _mediator;

    public CoverLettersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CoverLetterDto>>> GetAll(
        [FromQuery] Guid? profileId = null,
        [FromQuery] bool templatesOnly = false)
    {
        var result = await _mediator.Send(new GetAllCoverLettersQuery(profileId, templatesOnly));
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CoverLetterDto>> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetCoverLetterByIdQuery(id));
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<CoverLetterDto>> Create([FromBody] CreateCoverLetterCommandDto dto)
    {
        var command = new CreateCoverLetterCommand(
            dto.Title,
            dto.Body,
            dto.ResumeProfileId,
            dto.TargetCompany,
            dto.TargetRole,
            dto.RecipientName,
            dto.RecipientTitle,
            dto.IsTemplate);

        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CoverLetterDto>> Update(Guid id, [FromBody] UpdateCoverLetterCommandDto dto)
    {
        var command = new UpdateCoverLetterCommand(
            id,
            dto.Title,
            dto.Body,
            dto.ResumeProfileId,
            dto.TargetCompany,
            dto.TargetRole,
            dto.RecipientName,
            dto.RecipientTitle,
            dto.IsTemplate);

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteCoverLetterCommand(id));
        return NoContent();
    }
}

