using AutoMapper;
using CareerOS.Application.Features.Resumes.DTOs;
using CareerOS.Domain.Exceptions;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Resumes.Commands.UpdateResume;

public sealed class UpdateResumeCommandHandler : IRequestHandler<UpdateResumeCommand, ResumeDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public UpdateResumeCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<ResumeDto> Handle(UpdateResumeCommand request, CancellationToken cancellationToken)
    {
        var resume = await _uow.Resumes.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Resume", request.Id);

        resume.Update(request.Name, request.Content, request.IsDefault, request.Tags);

        await _uow.Resumes.UpdateAsync(resume, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return _mapper.Map<ResumeDto>(resume);
    }
}

