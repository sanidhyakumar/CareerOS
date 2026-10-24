using AutoMapper;
using CareerOS.Application.Features.Applications.DTOs;
using CareerOS.Domain.Enums;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Applications.Queries.GetAllApplications;

public record GetAllApplicationsQuery : IRequest<List<JobApplicationDto>>
{
    public Guid? JobId { get; init; }
    public Guid? CompanyId { get; init; }
    public ApplicationStatus? Status { get; init; }
    public string? SearchTerm { get; init; }
}

public class GetAllApplicationsQueryHandler : IRequestHandler<GetAllApplicationsQuery, List<JobApplicationDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetAllApplicationsQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<List<JobApplicationDto>> Handle(GetAllApplicationsQuery request, CancellationToken cancellationToken)
    {
        var applications = await _uow.Applications.GetAllAsync(
            jobId: request.JobId,
            companyId: request.CompanyId,
            status: request.Status,
            searchTerm: request.SearchTerm,
            cancellationToken: cancellationToken);

        return _mapper.Map<List<JobApplicationDto>>(applications);
    }
}

