using AutoMapper;
using CareerOS.Application.Features.Resumes.DTOs;
using CareerOS.Domain.Exceptions;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Resumes.Queries.GetResumeById;

public sealed class GetResumeByIdQueryHandler : IRequestHandler<GetResumeByIdQuery, ResumeDto?>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetResumeByIdQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<ResumeDto?> Handle(GetResumeByIdQuery request, CancellationToken cancellationToken)
    {
        var resume = await _uow.Resumes.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Resume", request.Id);
        return _mapper.Map<ResumeDto>(resume);
    }
}

