using AutoMapper;
using CareerOS.Application.Features.Interviews.DTOs;
using CareerOS.Domain.Entities;
using CareerOS.Domain.Exceptions;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Interviews.Commands.UpdateInterview;

public class UpdateInterviewCommandHandler : IRequestHandler<UpdateInterviewCommand, InterviewDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public UpdateInterviewCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<InterviewDto> Handle(UpdateInterviewCommand request, CancellationToken cancellationToken)
    {
        var interview = await _uow.Interviews.GetByIdAsync(request.Id, includeDetails: true, cancellationToken: cancellationToken)
            ?? throw new NotFoundException(nameof(Interview), request.Id);

        interview.Update(
            title: request.Title,
            interviewType: request.InterviewType,
            scheduledAt: request.ScheduledAt,
            durationMinutes: request.DurationMinutes,
            format: request.Format,
            status: request.Status,
            outcome: request.Outcome,
            jobApplicationId: request.JobApplicationId,
            jobId: request.JobId,
            companyId: request.CompanyId,
            meetingLink: request.MeetingLink,
            location: request.Location,
            interviewerNames: request.InterviewerNames,
            preparationNotes: request.PreparationNotes,
            questionsAsked: request.QuestionsAsked,
            feedbackNotes: request.FeedbackNotes,
            rating: request.Rating);

        if (request.InterviewerContactIds != null)
        {
            var links = request.InterviewerContactIds.Select(cid => (cid, (string?)"Interviewer"));
            await _uow.Interviews.SetInterviewersAsync(interview.Id, links, cancellationToken);
        }

        await _uow.Interviews.UpdateAsync(interview, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        var updated = await _uow.Interviews.GetByIdAsync(interview.Id, includeDetails: true, cancellationToken: cancellationToken);
        return _mapper.Map<InterviewDto>(updated ?? interview);
    }
}

