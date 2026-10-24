using AutoMapper;
using CareerOS.Application.Features.Jobs.DTOs;
using CareerOS.Domain.Exceptions;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Jobs.Queries.GetJobById;

public sealed class GetJobByIdQueryHandler : IRequestHandler<GetJobByIdQuery, JobDto?>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetJobByIdQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<JobDto?> Handle(GetJobByIdQuery request, CancellationToken cancellationToken)
    {
        var job = await _uow.Jobs.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Job", request.Id);
        return _mapper.Map<JobDto>(job);
    }
}

