using AutoMapper;
using CareerOS.Application.Features.Interviews.DTOs;
using CareerOS.Domain.Enums;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Interviews.Queries.GetAllInterviews;

public record GetAllInterviewsQuery : IRequest<List<InterviewDto>>
{
    public Guid? ApplicationId { get; init; }
    public Guid? CompanyId { get; init; }
    public Guid? JobId { get; init; }
    public InterviewType? Type { get; init; }
    public InterviewStatus? Status { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public string? SearchTerm { get; init; }
}

public class GetAllInterviewsQueryHandler : IRequestHandler<GetAllInterviewsQuery, List<InterviewDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetAllInterviewsQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<List<InterviewDto>> Handle(GetAllInterviewsQuery request, CancellationToken cancellationToken)
    {
        var interviews = await _uow.Interviews.GetAllAsync(
            applicationId: request.ApplicationId,
            companyId: request.CompanyId,
            jobId: request.JobId,
            type: request.Type,
            status: request.Status,
            fromDate: request.FromDate,
            toDate: request.ToDate,
            searchTerm: request.SearchTerm,
            cancellationToken: cancellationToken);

        return _mapper.Map<List<InterviewDto>>(interviews);
    }
}

