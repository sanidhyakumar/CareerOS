using AutoMapper;
using CareerOS.Application.Features.Applications.DTOs;
using CareerOS.Domain.Entities;
using CareerOS.Domain.Exceptions;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Applications.Commands.AddTimelineEvent;

public class AddTimelineEventCommandHandler : IRequestHandler<AddTimelineEventCommand, ApplicationTimelineEventDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public AddTimelineEventCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<ApplicationTimelineEventDto> Handle(AddTimelineEventCommand request, CancellationToken cancellationToken)
    {
        var application = await _uow.Applications.GetByIdAsync(request.ApplicationId, includeDetails: false, cancellationToken: cancellationToken);
        if (application == null)
            throw new NotFoundException(nameof(JobApplication), request.ApplicationId);

        var timelineEvent = ApplicationTimelineEvent.Create(
            jobApplicationId: request.ApplicationId,
            status: request.Status,
            eventDate: request.EventDate,
            title: request.Title,
            notes: request.Notes);

        await _uow.Applications.AddTimelineEventAsync(timelineEvent, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ApplicationTimelineEventDto>(timelineEvent);
    }
}

