using AutoMapper;
using CareerOS.Application.Features.Contacts.DTOs;
using CareerOS.Domain.Entities;
using CareerOS.Domain.Exceptions;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Contacts.Commands.Interactions;

public record AddContactInteractionCommand(
    Guid ContactId,
    string Summary,
    DateTime InteractionDate,
    string Channel = "Email",
    string? Notes = null,
    bool FollowUpRequired = false,
    DateTime? FollowUpDate = null
) : IRequest<ContactInteractionDto>;

public class AddContactInteractionCommandHandler : IRequestHandler<AddContactInteractionCommand, ContactInteractionDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public AddContactInteractionCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<ContactInteractionDto> Handle(AddContactInteractionCommand request, CancellationToken cancellationToken)
    {
        var contact = await _uow.Contacts.GetByIdAsync(request.ContactId, includeInteractions: true, cancellationToken: cancellationToken)
            ?? throw new NotFoundException(nameof(Contact), request.ContactId);

        var interaction = ContactInteraction.Create(
            contactId: request.ContactId,
            summary: request.Summary,
            interactionDate: request.InteractionDate,
            channel: request.Channel,
            notes: request.Notes,
            followUpRequired: request.FollowUpRequired,
            followUpDate: request.FollowUpDate);

        contact.LastContactDate = request.InteractionDate;
        if (request.FollowUpRequired && request.FollowUpDate.HasValue)
        {
            contact.NextFollowUpDate = request.FollowUpDate;
        }

        await _uow.Contacts.AddInteractionAsync(interaction, cancellationToken);
        await _uow.Contacts.UpdateAsync(contact, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ContactInteractionDto>(interaction);
    }
}

