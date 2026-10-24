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
        string companyName = request.Company;
        if (request.CompanyId.HasValue && string.IsNullOrWhiteSpace(companyName))
        {
            var company = await _uow.Companies.GetByIdAsync(request.CompanyId.Value, cancellationToken: cancellationToken);
            if (company != null)
            {
                companyName = company.Name;
            }
        }

        var job = Job.Create(
            request.Title,
            companyName,
            request.CompanyId,
            request.Location,
            request.Url,
            request.Priority,
            request.EmploymentType,
            request.WorkplaceType,
            request.Seniority,
            request.MinSalary,
            request.MaxSalary,
            request.Currency,
            request.Salary,
            request.Equity,
            request.Department,
            request.TechStack,
            request.Deadline,
            request.Description,
            request.Notes);

        job.Status = request.Status;
        job.AppliedAt = request.AppliedAt;
        job.InterviewAt = request.InterviewAt;

        await _uow.Jobs.AddAsync(job, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        var reloaded = await _uow.Jobs.GetByIdAsync(job.Id, cancellationToken);
        return _mapper.Map<JobDto>(reloaded ?? job);
    }
}
