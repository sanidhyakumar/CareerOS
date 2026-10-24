using AutoMapper;
using CareerOS.Application.Features.Jobs.DTOs;
using CareerOS.Domain.Exceptions;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Jobs.Commands.UpdateJob;

public sealed class UpdateJobCommandHandler : IRequestHandler<UpdateJobCommand, JobDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public UpdateJobCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<JobDto> Handle(UpdateJobCommand request, CancellationToken cancellationToken)
    {
        var job = await _uow.Jobs.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Job", request.Id);

        job.Update(request.Title, request.Company, request.Location, request.Url, request.Description,
            request.Notes, request.Status, request.Salary, request.AppliedAt, request.InterviewAt);

        await _uow.Jobs.UpdateAsync(job, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return _mapper.Map<JobDto>(job);
    }
}

