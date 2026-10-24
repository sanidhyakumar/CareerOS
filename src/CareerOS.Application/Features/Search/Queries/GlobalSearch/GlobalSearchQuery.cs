using System.Diagnostics;
using CareerOS.Application.Features.Search.DTOs;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Search.Queries.GlobalSearch;

public record GlobalSearchQuery(
    string Query,
    string? EntityType = null,
    int Limit = 30
) : IRequest<SearchSummaryDto>;

public class GlobalSearchQueryHandler : IRequestHandler<GlobalSearchQuery, SearchSummaryDto>
{
    private readonly ISearchService _searchService;

    public GlobalSearchQueryHandler(ISearchService searchService)
    {
        _searchService = searchService;
    }

    public async Task<SearchSummaryDto> Handle(GlobalSearchQuery request, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        if (string.IsNullOrWhiteSpace(request.Query))
        {
            return new SearchSummaryDto
            {
                Query = string.Empty,
                TotalMatches = 0,
                ElapsedMilliseconds = 0,
                MatchesByType = new(),
                Results = new()
            };
        }

        var results = await _searchService.SearchAsync(
            request.Query,
            request.EntityType,
            request.Limit,
            cancellationToken);

        stopwatch.Stop();

        var matchesByType = results
            .GroupBy(r => r.EntityType)
            .ToDictionary(g => g.Key, g => g.Count());

        var dtos = results.Select(r => new SearchResultDto
        {
            EntityId = r.EntityId,
            EntityType = r.EntityType,
            Title = r.Title,
            Subtitle = r.Subtitle,
            Snippet = r.Snippet,
            ActionUrl = r.ActionUrl,
            Tags = r.Tags,
            Score = r.Score
        }).ToList();

        return new SearchSummaryDto
        {
            Query = request.Query,
            TotalMatches = dtos.Count,
            ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
            MatchesByType = matchesByType,
            Results = dtos
        };
    }
}
