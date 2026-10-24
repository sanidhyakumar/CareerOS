using AutoMapper;
using CareerOS.Application.Features.Applications.DTOs;
using CareerOS.Domain.Entities;
using CareerOS.Domain.Exceptions;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Applications.Queries.GetApplicationById;

public record GetApplicationByIdQuery(Guid Id) : IRequest<JobApplicationDto>;

public class GetApplicationByIdQueryHandler : IRequestHandler<GetApplicationByIdQuery, JobApplicationDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetApplicationByIdQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<JobApplicationDto> Handle(GetApplicationByIdQuery request, CancellationToken cancellationToken)
    {
        var application = await _uow.Applications.GetByIdAsync(request.Id, includeDetails: true, cancellationToken: cancellationToken);
        if (application == null)
            throw new NotFoundException(nameof(JobApplication), request.Id);

        return _mapper.Map<JobApplicationDto>(application);
    }
}
