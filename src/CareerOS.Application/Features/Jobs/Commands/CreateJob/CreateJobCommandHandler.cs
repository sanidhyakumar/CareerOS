using AutoMapper;
using CareerOS.Application.Features.Jobs.DTOs;
using CareerOS.Domain.Entities;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Jobs.Commands.CreateJob;

public sealed class CreateJobCommandHandler : IRequestHandler<CreateJobCommand, JobDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CreateJobCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<JobDto> Handle(CreateJobCommand request, CancellationToken cancellationToken)
    {
        var job = Job.Create(request.Title, request.Company, request.Location, request.Url);
        job.Notes = request.Notes;
        job.Description = request.Description;
        job.Status = request.Status;
        job.Salary = request.Salary;

        await _uow.Jobs.AddAsync(job, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return _mapper.Map<JobDto>(job);
    }
}

