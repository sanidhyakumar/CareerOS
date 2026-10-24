using AutoMapper;
using CareerOS.Application.Features.Applications.DTOs;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Applications.Queries.GetApplicationTimeline;

public record GetApplicationTimelineQuery(Guid ApplicationId) : IRequest<List<ApplicationTimelineEventDto>>;

public class GetApplicationTimelineQueryHandler : IRequestHandler<GetApplicationTimelineQuery, List<ApplicationTimelineEventDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetApplicationTimelineQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<List<ApplicationTimelineEventDto>> Handle(GetApplicationTimelineQuery request, CancellationToken cancellationToken)
    {
        var events = await _uow.Applications.GetTimelineEventsAsync(request.ApplicationId, cancellationToken);
        return _mapper.Map<List<ApplicationTimelineEventDto>>(events);
    }
}
