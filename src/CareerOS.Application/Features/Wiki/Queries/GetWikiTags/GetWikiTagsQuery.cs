using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Wiki.Queries.GetWikiTags;

public record GetWikiTagsQuery : IRequest<List<string>>;

public class GetWikiTagsQueryHandler : IRequestHandler<GetWikiTagsQuery, List<string>>
{
    private readonly IUnitOfWork _uow;

    public GetWikiTagsQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<List<string>> Handle(GetWikiTagsQuery request, CancellationToken cancellationToken)
    {
        return await _uow.Wiki.GetTagsAsync(cancellationToken);
    }
}
