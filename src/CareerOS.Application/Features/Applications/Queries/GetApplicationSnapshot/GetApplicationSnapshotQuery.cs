using AutoMapper;
using CareerOS.Application.Features.Applications.DTOs;
using CareerOS.Domain.Exceptions;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Applications.Queries.GetApplicationSnapshot;

public record GetApplicationSnapshotQuery(Guid ApplicationId) : IRequest<ApplicationSnapshotDto>;

public class GetApplicationSnapshotQueryHandler : IRequestHandler<GetApplicationSnapshotQuery, ApplicationSnapshotDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetApplicationSnapshotQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<ApplicationSnapshotDto> Handle(GetApplicationSnapshotQuery request, CancellationToken cancellationToken)
    {
        var snapshot = await _uow.Applications.GetSnapshotAsync(request.ApplicationId, cancellationToken);
        if (snapshot == null)
            throw new NotFoundException("ApplicationSnapshot", request.ApplicationId);

        return _mapper.Map<ApplicationSnapshotDto>(snapshot);
    }
}
