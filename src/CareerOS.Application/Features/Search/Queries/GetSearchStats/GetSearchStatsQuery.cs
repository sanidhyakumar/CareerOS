using CareerOS.Application.Features.Search.DTOs;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Search.Queries.GetSearchStats;

public record GetSearchStatsQuery : IRequest<SearchStatsDto>;

public class GetSearchStatsQueryHandler : IRequestHandler<GetSearchStatsQuery, SearchStatsDto>
{
    private readonly ISearchService _searchService;

    public GetSearchStatsQueryHandler(ISearchService searchService)
    {
        _searchService = searchService;
    }

    public async Task<SearchStatsDto> Handle(GetSearchStatsQuery request, CancellationToken cancellationToken)
    {
        var stats = await _searchService.GetSearchStatsAsync(cancellationToken);
        int total = stats.Values.Sum();

        return new SearchStatsDto
        {
            TotalIndexedRecords = total,
            RecordsByType = new Dictionary<string, int>(stats),
            LastIndexedAt = DateTime.UtcNow
        };
    }
}
