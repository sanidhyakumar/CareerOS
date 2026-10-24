using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Search.Commands.ReindexAll;

public record ReindexAllCommand : IRequest<int>;

public class ReindexAllCommandHandler : IRequestHandler<ReindexAllCommand, int>
{
    private readonly ISearchService _searchService;

    public ReindexAllCommandHandler(ISearchService searchService)
    {
        _searchService = searchService;
    }

    public async Task<int> Handle(ReindexAllCommand request, CancellationToken cancellationToken)
    {
        return await _searchService.ReindexAllAsync(cancellationToken);
    }
}

