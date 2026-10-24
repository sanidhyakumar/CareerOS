using CareerOS.Application.Features.FollowUp.Commands.CreateCustomReminder;
using CareerOS.Application.Features.FollowUp.Commands.DeleteReminder;
using CareerOS.Application.Features.FollowUp.Commands.DismissReminder;
using CareerOS.Application.Features.FollowUp.Commands.SnoozeReminder;
using CareerOS.Application.Features.FollowUp.Commands.TriggerFollowUpScan;
using CareerOS.Application.Features.FollowUp.DTOs;
using CareerOS.Application.Features.FollowUp.Queries.GetFollowUpSummary;
using CareerOS.Application.Features.FollowUp.Queries.GetReminders;
using CareerOS.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CareerOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FollowUpController : ControllerBase
{
    private readonly IMediator _mediator;

    public FollowUpController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<ReminderDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] ReminderCategory? category,
        [FromQuery] ReminderSeverity? severity,
        [FromQuery] bool includeDismissed = false,
        [FromQuery] bool includeSnoozed = false,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetRemindersQuery(
            Category: category,
            Severity: severity,
            IncludeDismissed: includeDismissed,
            IncludeSnoozed: includeSnoozed), cancellationToken);

        return Ok(result);
    }

    [HttpGet("summary")]
    [ProducesResponseType(typeof(FollowUpSummaryDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSummary(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetFollowUpSummaryQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPost("scan")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public async Task<IActionResult> RunScan(CancellationToken cancellationToken)
    {
        var createdCount = await _mediator.Send(new TriggerFollowUpScanCommand(), cancellationToken);
        return Ok(new { newRemindersCount = createdCount });
    }

    [HttpPost("custom")]
    [ProducesResponseType(typeof(ReminderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCustom([FromBody] CreateCustomReminderCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/dismiss")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Dismiss(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DismissReminderCommand(id), cancellationToken);
        return NoContent();
    }

    public record SnoozeRequest(int Days);

    [HttpPatch("{id:guid}/snooze")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Snooze(Guid id, [FromBody] SnoozeRequest request, CancellationToken cancellationToken)
    {
        await _mediator.Send(new SnoozeReminderCommand(id, request.Days), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteReminderCommand(id), cancellationToken);
        return NoContent();
    }
}
