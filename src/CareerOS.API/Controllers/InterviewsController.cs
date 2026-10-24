using CareerOS.Application.Features.Interviews.Commands.CreateInterview;
using CareerOS.Application.Features.Interviews.Commands.DeleteInterview;
using CareerOS.Application.Features.Interviews.Commands.UpdateInterview;
using CareerOS.Application.Features.Interviews.Commands.UpdateInterviewStatus;
using CareerOS.Application.Features.Interviews.DTOs;
using CareerOS.Application.Features.Interviews.Queries.GetAllInterviews;
using CareerOS.Application.Features.Interviews.Queries.GetInterviewById;
using CareerOS.Application.Features.Interviews.Queries.GetInterviewStats;
using CareerOS.Application.Features.Interviews.Queries.GetUpcomingInterviews;
using CareerOS.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CareerOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InterviewsController : ControllerBase
{
    private readonly IMediator _mediator;

    public InterviewsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<InterviewDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? applicationId,
        [FromQuery] Guid? companyId,
        [FromQuery] Guid? jobId,
        [FromQuery] InterviewType? type,
        [FromQuery] InterviewStatus? status,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] string? searchTerm,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllInterviewsQuery
        {
            ApplicationId = applicationId,
            CompanyId = companyId,
            JobId = jobId,
            Type = type,
            Status = status,
            FromDate = fromDate,
            ToDate = toDate,
            SearchTerm = searchTerm
        }, cancellationToken);

        return Ok(result);
    }

    [HttpGet("stats")]
    [ProducesResponseType(typeof(InterviewStatsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStats(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetInterviewStatsQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("upcoming")]
    [ProducesResponseType(typeof(List<InterviewDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUpcoming([FromQuery] int count = 10, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetUpcomingInterviewsQuery(count), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(InterviewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetInterviewByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(InterviewDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateInterviewCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(InterviewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateInterviewCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest("ID in URL does not match body.");

        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(InterviewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateInterviewStatusRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateInterviewStatusCommand
        {
            Id = id,
            Status = request.Status,
            Outcome = request.Outcome,
            FeedbackNotes = request.FeedbackNotes,
            Rating = request.Rating
        };

        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteInterviewCommand(id), cancellationToken);
        return NoContent();
    }
}

public class UpdateInterviewStatusRequest
{
    public InterviewStatus Status { get; set; }
    public InterviewOutcome Outcome { get; set; }
    public string? FeedbackNotes { get; set; }
    public int? Rating { get; set; }
}
