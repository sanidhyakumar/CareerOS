using AutoMapper;
using CareerOS.Application.Features.Interviews.DTOs;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Interviews.Queries.GetUpcomingInterviews;

public record GetUpcomingInterviewsQuery(int Count = 10) : IRequest<List<InterviewDto>>;

public class GetUpcomingInterviewsQueryHandler : IRequestHandler<GetUpcomingInterviewsQuery, List<InterviewDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetUpcomingInterviewsQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<List<InterviewDto>> Handle(GetUpcomingInterviewsQuery request, CancellationToken cancellationToken)
    {
        var items = await _uow.Interviews.GetUpcomingAsync(request.Count, cancellationToken);
        return _mapper.Map<List<InterviewDto>>(items);
    }
}

