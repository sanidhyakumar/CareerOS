using CareerOS.Application.Features.Wiki.DTOs;
using CareerOS.Domain.Enums;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Wiki.Queries.GetWikiStats;

public record GetWikiStatsQuery : IRequest<WikiStatsDto>;

public class GetWikiStatsQueryHandler : IRequestHandler<GetWikiStatsQuery, WikiStatsDto>
{
    private readonly IUnitOfWork _uow;

    public GetWikiStatsQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<WikiStatsDto> Handle(GetWikiStatsQuery request, CancellationToken cancellationToken)
    {
        var articles = (await _uow.Wiki.GetAllAsync(cancellationToken: cancellationToken)).ToList();
        var tags = await _uow.Wiki.GetTagsAsync(cancellationToken);

        return new WikiStatsDto
        {
            TotalArticles = articles.Count,
            StarStoriesCount = articles.Count(a => a.Category == WikiCategory.StarStory),
            SystemDesignCount = articles.Count(a => a.Category == WikiCategory.SystemDesign),
            CompanyResearchCount = articles.Count(a => a.Category == WikiCategory.CompanyResearch),
            InterviewNotesCount = articles.Count(a => a.Category == WikiCategory.InterviewNotes),
            StarredCount = articles.Count(a => a.IsStarred),
            TagsCount = tags.Count
        };
    }
}

