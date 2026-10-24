using AutoMapper;
using CareerOS.Application.Features.Resumes.DTOs;
using CareerOS.Domain.Entities;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Resumes.Commands.CreateResume;

public sealed class CreateResumeCommandHandler : IRequestHandler<CreateResumeCommand, ResumeDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CreateResumeCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<ResumeDto> Handle(CreateResumeCommand request, CancellationToken cancellationToken)
    {
        var resume = Resume.Create(request.Name, request.Content);
        resume.IsDefault = request.IsDefault;
        resume.Tags = request.Tags;

        await _uow.Resumes.AddAsync(resume, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return _mapper.Map<ResumeDto>(resume);
    }
}

