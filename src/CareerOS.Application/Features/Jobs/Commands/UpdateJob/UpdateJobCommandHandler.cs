using AutoMapper;
using CareerOS.Application.Features.Jobs.DTOs;
using CareerOS.Domain.Exceptions;
using CareerOS.Domain.Entities;
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
        var job = await _uow.Jobs.GetByIdAsync(request.Id, cancellationToken);
        if (job is null)
            throw new NotFoundException(nameof(Job), request.Id);

        string companyName = request.Company;
        if (request.CompanyId.HasValue && string.IsNullOrWhiteSpace(companyName))
        {
            var company = await _uow.Companies.GetByIdAsync(request.CompanyId.Value, cancellationToken: cancellationToken);
            if (company != null)
            {
                companyName = company.Name;
            }
        }

        job.Update(
            request.Title,
            companyName,
            request.CompanyId,
            request.Department,
            request.Location,
            request.Url,
            request.Description,
            request.Notes,
            request.Status,
            request.Priority,
            request.EmploymentType,
            request.WorkplaceType,
            request.Seniority,
            request.MinSalary,
            request.MaxSalary,
            request.Currency,
            request.Salary,
            request.Equity,
            request.TechStack,
            request.Deadline,
            request.AppliedAt,
            request.InterviewAt);

        await _uow.Jobs.UpdateAsync(job, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        var reloaded = await _uow.Jobs.GetByIdAsync(job.Id, cancellationToken);
        return _mapper.Map<JobDto>(reloaded ?? job);
    }
}
