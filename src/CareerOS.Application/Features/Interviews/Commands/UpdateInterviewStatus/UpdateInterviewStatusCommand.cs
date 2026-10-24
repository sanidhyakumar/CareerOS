using CareerOS.Application.Features.Interviews.DTOs;
using CareerOS.Domain.Entities;
using CareerOS.Domain.Enums;
using CareerOS.Domain.Exceptions;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Interviews.Commands.UpdateInterviewStatus;

public record UpdateInterviewStatusCommand : IRequest<InterviewDto>
{
    public Guid Id { get; init; }
    public InterviewStatus Status { get; init; }
    public InterviewOutcome Outcome { get; init; }
    public string? FeedbackNotes { get; init; }
    public int? Rating { get; init; }
}

public class UpdateInterviewStatusCommandHandler : IRequestHandler<UpdateInterviewStatusCommand, InterviewDto>
{
    private readonly IUnitOfWork _uow;
    private readonly AutoMapper.IMapper _mapper;

    public UpdateInterviewStatusCommandHandler(IUnitOfWork uow, AutoMapper.IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<InterviewDto> Handle(UpdateInterviewStatusCommand request, CancellationToken cancellationToken)
    {
        var interview = await _uow.Interviews.GetByIdAsync(request.Id, includeDetails: true, cancellationToken: cancellationToken)
            ?? throw new NotFoundException(nameof(Interview), request.Id);

        interview.Status = request.Status;
        interview.Outcome = request.Outcome;
        if (!string.IsNullOrWhiteSpace(request.FeedbackNotes))
        {
            interview.FeedbackNotes = request.FeedbackNotes.Trim();
        }
        if (request.Rating.HasValue)
        {
            interview.Rating = request.Rating.Value;
        }
        interview.UpdatedAt = DateTime.UtcNow;

        // If completed with outcome and linked to an application, log timeline event
        if (interview.JobApplicationId.HasValue && request.Status == InterviewStatus.Completed)
        {
            var timelineEvent = ApplicationTimelineEvent.Create(
                jobApplicationId: interview.JobApplicationId.Value,
                status: ApplicationStatus.TechnicalInterview,
                eventDate: DateTime.UtcNow,
                title: $"{interview.InterviewType} Interview Completed ({request.Outcome})",
                notes: request.FeedbackNotes);

            await _uow.Applications.AddTimelineEventAsync(timelineEvent, cancellationToken);
        }

        await _uow.Interviews.UpdateAsync(interview, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        var updated = await _uow.Interviews.GetByIdAsync(interview.Id, includeDetails: true, cancellationToken: cancellationToken);
        return _mapper.Map<InterviewDto>(updated ?? interview);
    }
}

