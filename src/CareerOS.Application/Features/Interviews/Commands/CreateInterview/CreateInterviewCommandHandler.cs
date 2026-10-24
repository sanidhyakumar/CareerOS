using AutoMapper;
using CareerOS.Application.Features.Interviews.DTOs;
using CareerOS.Domain.Entities;
using CareerOS.Domain.Enums;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Interviews.Commands.CreateInterview;

public class CreateInterviewCommandHandler : IRequestHandler<CreateInterviewCommand, InterviewDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CreateInterviewCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<InterviewDto> Handle(CreateInterviewCommand request, CancellationToken cancellationToken)
    {
        Guid? jobId = request.JobId;
        Guid? companyId = request.CompanyId;

        // Auto-resolve Job and Company from JobApplication if provided
        if (request.JobApplicationId.HasValue)
        {
            var app = await _uow.Applications.GetByIdAsync(request.JobApplicationId.Value, includeDetails: false, cancellationToken: cancellationToken);
            if (app != null)
            {
                jobId ??= app.JobId;
                companyId ??= app.CompanyId;
            }
        }

        if (jobId.HasValue && !companyId.HasValue)
        {
            var job = await _uow.Jobs.GetByIdAsync(jobId.Value, cancellationToken: cancellationToken);
            if (job != null)
            {
                companyId = job.CompanyId;
            }
        }

        var interview = Interview.Create(
            title: request.Title,
            interviewType: request.InterviewType,
            scheduledAt: request.ScheduledAt,
            durationMinutes: request.DurationMinutes,
            format: request.Format,
            jobApplicationId: request.JobApplicationId,
            jobId: jobId,
            companyId: companyId,
            meetingLink: request.MeetingLink,
            location: request.Location,
            interviewerNames: request.InterviewerNames,
            preparationNotes: request.PreparationNotes);

        await _uow.Interviews.AddAsync(interview, cancellationToken);

        // Associate Interviewer Contacts if provided
        if (request.InterviewerContactIds != null && request.InterviewerContactIds.Any())
        {
            var links = request.InterviewerContactIds.Select(cid => (cid, (string?)"Interviewer"));
            await _uow.Interviews.SetInterviewersAsync(interview.Id, links, cancellationToken);
        }

        // Add application timeline event if linked
        if (request.JobApplicationId.HasValue)
        {
            var timelineEvent = ApplicationTimelineEvent.Create(
                jobApplicationId: request.JobApplicationId.Value,
                status: ApplicationStatus.TechnicalInterview,
                eventDate: request.ScheduledAt,
                title: $"{request.InterviewType} Interview Scheduled: {request.Title}",
                notes: request.PreparationNotes);

            await _uow.Applications.AddTimelineEventAsync(timelineEvent, cancellationToken);
        }

        await _uow.SaveChangesAsync(cancellationToken);

        var full = await _uow.Interviews.GetByIdAsync(interview.Id, includeDetails: true, cancellationToken: cancellationToken);
        return _mapper.Map<InterviewDto>(full ?? interview);
    }
}
