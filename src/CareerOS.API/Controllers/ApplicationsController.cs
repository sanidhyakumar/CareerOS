using CareerOS.Application.Features.Applications.Commands.AddTimelineEvent;
using CareerOS.Application.Features.Applications.Commands.CreateApplication;
using CareerOS.Application.Features.Applications.Commands.DeleteApplication;
using CareerOS.Application.Features.Applications.Commands.UpdateApplication;
using CareerOS.Application.Features.Applications.Commands.UpdateApplicationStatus;
using CareerOS.Application.Features.Applications.DTOs;
using CareerOS.Application.Features.Applications.Queries.GetAllApplications;
using CareerOS.Application.Features.Applications.Queries.GetApplicationById;
using CareerOS.Application.Features.Applications.Queries.GetApplicationSnapshot;
using CareerOS.Application.Features.Applications.Queries.GetApplicationStats;
using CareerOS.Application.Features.Applications.Queries.GetApplicationTimeline;
using CareerOS.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CareerOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApplicationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ApplicationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<JobApplicationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? jobId,
        [FromQuery] Guid? companyId,
        [FromQuery] ApplicationStatus? status,
        [FromQuery] string? searchTerm,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllApplicationsQuery
        {
            JobId = jobId,
            CompanyId = companyId,
            Status = status,
            SearchTerm = searchTerm
        }, cancellationToken);

        return Ok(result);
    }

    [HttpGet("stats")]
    [ProducesResponseType(typeof(ApplicationStatsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStats(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetApplicationStatsQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(JobApplicationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetApplicationByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(JobApplicationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateApplicationCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(JobApplicationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateApplicationCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest("ID in URL does not match body.");

        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(JobApplicationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateStatusRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateApplicationStatusCommand
        {
            Id = id,
            NewStatus = request.NewStatus,
            EventTitle = request.EventTitle,
            Notes = request.Notes
        };

        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}/timeline")]
    [ProducesResponseType(typeof(List<ApplicationTimelineEventDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTimeline(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetApplicationTimelineQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpPost("{id:guid}/timeline")]
    [ProducesResponseType(typeof(ApplicationTimelineEventDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddTimelineEvent(Guid id, [FromBody] AddTimelineEventRequest request, CancellationToken cancellationToken)
    {
        var command = new AddTimelineEventCommand
        {
            ApplicationId = id,
            Status = request.Status,
            EventDate = request.EventDate ?? DateTime.UtcNow,
            Title = request.Title,
            Notes = request.Notes
        };

        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetTimeline), new { id }, result);
    }

    [HttpGet("{id:guid}/snapshot")]
    [ProducesResponseType(typeof(ApplicationSnapshotDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSnapshot(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetApplicationSnapshotQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteApplicationCommand(id), cancellationToken);
        return NoContent();
    }
}

public class UpdateStatusRequest
{
    public ApplicationStatus NewStatus { get; set; }
    public string? EventTitle { get; set; }
    public string? Notes { get; set; }
}

public class AddTimelineEventRequest
{
    public ApplicationStatus Status { get; set; }
    public DateTime? EventDate { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

