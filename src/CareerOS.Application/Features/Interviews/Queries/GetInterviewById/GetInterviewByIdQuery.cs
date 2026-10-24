using AutoMapper;
using CareerOS.Application.Features.Interviews.DTOs;
using CareerOS.Domain.Entities;
using CareerOS.Domain.Exceptions;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Interviews.Queries.GetInterviewById;

public record GetInterviewByIdQuery(Guid Id) : IRequest<InterviewDto>;

public class GetInterviewByIdQueryHandler : IRequestHandler<GetInterviewByIdQuery, InterviewDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetInterviewByIdQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<InterviewDto> Handle(GetInterviewByIdQuery request, CancellationToken cancellationToken)
    {
        var interview = await _uow.Interviews.GetByIdAsync(request.Id, includeDetails: true, cancellationToken: cancellationToken)
            ?? throw new NotFoundException(nameof(Interview), request.Id);

        return _mapper.Map<InterviewDto>(interview);
    }
}
