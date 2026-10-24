using CareerOS.Application.Features.Kanban.Commands.CreateKanbanCard;
using CareerOS.Application.Features.Kanban.Commands.DeleteKanbanCard;
using CareerOS.Application.Features.Kanban.Commands.MoveKanbanCard;
using CareerOS.Application.Features.Kanban.Commands.UpdateKanbanCard;
using CareerOS.Application.Features.Kanban.DTOs;
using CareerOS.Application.Features.Kanban.Queries.GetKanbanBoard;
using CareerOS.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CareerOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class KanbanController : ControllerBase
{
    private readonly IMediator _mediator;

    public KanbanController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(KanbanBoardDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBoard(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetKanbanBoardQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPost("cards")]
    [ProducesResponseType(typeof(KanbanCardDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCard([FromBody] CreateKanbanCardCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPut("cards/{id:guid}")]
    [ProducesResponseType(typeof(KanbanCardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCard(Guid id, [FromBody] UpdateKanbanCardCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            return BadRequest("ID in path does not match ID in body.");
        }

        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    public record MoveCardRequest(KanbanColumnType TargetColumn, int NewPosition);

    [HttpPatch("cards/{id:guid}/move")]
    [ProducesResponseType(typeof(KanbanCardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MoveCard(Guid id, [FromBody] MoveCardRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new MoveKanbanCardCommand(
            CardId: id,
            TargetColumn: request.TargetColumn,
            NewPosition: request.NewPosition), cancellationToken);

        return Ok(result);
    }

    [HttpDelete("cards/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCard(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteKanbanCardCommand(id), cancellationToken);
        return NoContent();
    }
}

