using AutoMapper;
using CareerOS.Application.Features.Resumes.DTOs;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Resumes.Queries.GetAllResumes;

public sealed class GetAllResumesQueryHandler : IRequestHandler<GetAllResumesQuery, IEnumerable<ResumeDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetAllResumesQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ResumeDto>> Handle(GetAllResumesQuery request, CancellationToken cancellationToken)
    {
        var resumes = await _uow.Resumes.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<ResumeDto>>(resumes);
    }
}

