using AutoMapper;
using CareerOS.Application.Features.Applications.DTOs;
using CareerOS.Domain.Entities;
using CareerOS.Domain.Exceptions;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Applications.Commands.UpdateApplication;

public class UpdateApplicationCommandHandler : IRequestHandler<UpdateApplicationCommand, JobApplicationDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public UpdateApplicationCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<JobApplicationDto> Handle(UpdateApplicationCommand request, CancellationToken cancellationToken)
    {
        var application = await _uow.Applications.GetByIdAsync(request.Id, includeDetails: true, cancellationToken: cancellationToken);
        if (application == null)
            throw new NotFoundException(nameof(JobApplication), request.Id);

        var previousStatus = application.Status;

        application.Update(
            resumeVersionId: request.ResumeVersionId,
            coverLetterId: request.CoverLetterId,
            portfolioVersionId: request.PortfolioVersionId,
            applicationDate: request.ApplicationDate,
            status: request.Status,
            channel: request.Channel,
            referralContactId: request.ReferralContactId,
            notes: request.Notes);

        if (previousStatus != request.Status)
        {
            var timelineEvent = ApplicationTimelineEvent.Create(
                jobApplicationId: application.Id,
                status: request.Status,
                eventDate: DateTime.UtcNow,
                title: $"Stage updated: {previousStatus} → {request.Status}",
                notes: request.Notes);

            await _uow.Applications.AddTimelineEventAsync(timelineEvent, cancellationToken);
        }

        await _uow.Applications.UpdateAsync(application, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        var updated = await _uow.Applications.GetByIdAsync(application.Id, includeDetails: true, cancellationToken: cancellationToken);
        return _mapper.Map<JobApplicationDto>(updated ?? application);
    }
}
