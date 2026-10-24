using AutoMapper;
using CareerOS.Application.Features.Applications.DTOs;
using CareerOS.Domain.Entities;
using CareerOS.Domain.Exceptions;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Applications.Commands.UpdateApplicationStatus;

public class UpdateApplicationStatusCommandHandler : IRequestHandler<UpdateApplicationStatusCommand, JobApplicationDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public UpdateApplicationStatusCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<JobApplicationDto> Handle(UpdateApplicationStatusCommand request, CancellationToken cancellationToken)
    {
        var application = await _uow.Applications.GetByIdAsync(request.Id, includeDetails: true, cancellationToken: cancellationToken);
        if (application == null)
            throw new NotFoundException(nameof(JobApplication), request.Id);

        var previous = application.Status;
        application.Status = request.NewStatus;
        application.UpdatedAt = DateTime.UtcNow;

        string eventTitle = !string.IsNullOrWhiteSpace(request.EventTitle)
            ? request.EventTitle.Trim()
            : $"Status changed: {previous} → {request.NewStatus}";

        var timelineEvent = ApplicationTimelineEvent.Create(
            jobApplicationId: application.Id,
            status: request.NewStatus,
            eventDate: DateTime.UtcNow,
            title: eventTitle,
            notes: request.Notes);

        await _uow.Applications.AddTimelineEventAsync(timelineEvent, cancellationToken);
        await _uow.Applications.UpdateAsync(application, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        var updated = await _uow.Applications.GetByIdAsync(application.Id, includeDetails: true, cancellationToken: cancellationToken);
        return _mapper.Map<JobApplicationDto>(updated ?? application);
    }
}
