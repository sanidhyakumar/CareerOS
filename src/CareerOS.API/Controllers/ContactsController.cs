using CareerOS.Application.Features.Contacts.Commands.CreateContact;
using CareerOS.Application.Features.Contacts.Commands.DeleteContact;
using CareerOS.Application.Features.Contacts.Commands.Interactions;
using CareerOS.Application.Features.Contacts.Commands.UpdateContact;
using CareerOS.Application.Features.Contacts.DTOs;
using CareerOS.Application.Features.Contacts.Queries.GetAllContacts;
using CareerOS.Application.Features.Contacts.Queries.GetContactById;
using CareerOS.Application.Features.Contacts.Queries.GetContactStats;
using CareerOS.Domain.Enums;
using CareerOS.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CareerOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IUnitOfWork _uow;

    public ContactsController(IMediator mediator, IUnitOfWork uow)
    {
        _mediator = mediator;
        _uow = uow;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ContactDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? searchTerm,
        [FromQuery] ContactCategory? category,
        [FromQuery] Guid? companyId,
        [FromQuery] bool? isStarred,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllContactsQuery(
            SearchTerm: searchTerm,
            Category: category,
            CompanyId: companyId,
            IsStarred: isStarred), cancellationToken);

        return Ok(result);
    }

    [HttpGet("stats")]
    [ProducesResponseType(typeof(ContactStatsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStats(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetContactStatsQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ContactDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetContactByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ContactDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateContactCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ContactDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateContactCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest("ID in URL does not match body.");

        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/star")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleStar(Guid id, CancellationToken cancellationToken)
    {
        var contact = await _uow.Contacts.GetByIdAsync(id, includeInteractions: false, cancellationToken: cancellationToken);
        if (contact == null)
            return NotFound();

        contact.IsStarred = !contact.IsStarred;
        contact.UpdatedAt = DateTime.UtcNow;
        await _uow.Contacts.UpdateAsync(contact, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return Ok(new { isStarred = contact.IsStarred });
    }

    [HttpGet("{id:guid}/interactions")]
    [ProducesResponseType(typeof(IEnumerable<ContactInteractionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInteractions(Guid id, CancellationToken cancellationToken)
    {
        var contact = await _mediator.Send(new GetContactByIdQuery(id), cancellationToken);
        if (contact == null) return NotFound();
        return Ok(contact.Interactions);
    }

    [HttpPost("{id:guid}/interactions")]
    [ProducesResponseType(typeof(ContactInteractionDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> AddInteraction(Guid id, [FromBody] AddContactInteractionRequest request, CancellationToken cancellationToken)
    {
        var command = new AddContactInteractionCommand(
            ContactId: id,
            Summary: request.Summary,
            InteractionDate: request.InteractionDate ?? DateTime.UtcNow,
            Channel: request.Channel ?? "Email",
            Notes: request.Notes,
            FollowUpRequired: request.FollowUpRequired,
            FollowUpDate: request.FollowUpDate);

        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetInteractions), new { id }, result);
    }

    [HttpDelete("{id:guid}/interactions/{interactionId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteInteraction(Guid id, Guid interactionId, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteContactInteractionCommand(interactionId), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteContactCommand(id), cancellationToken);
        return NoContent();
    }
}

public class AddContactInteractionRequest
{
    public string Summary { get; set; } = string.Empty;
    public DateTime? InteractionDate { get; set; }
    public string? Channel { get; set; }
    public string? Notes { get; set; }
    public bool FollowUpRequired { get; set; }
    public DateTime? FollowUpDate { get; set; }
}
