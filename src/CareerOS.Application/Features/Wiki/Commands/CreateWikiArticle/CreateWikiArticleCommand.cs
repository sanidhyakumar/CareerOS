using CareerOS.Application.Features.Wiki.DTOs;
using CareerOS.Domain.Enums;
using MediatR;

namespace CareerOS.Application.Features.Wiki.Commands.CreateWikiArticle;

public record CreateWikiArticleCommand : IRequest<WikiArticleDto>
{
    public string Title { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public WikiCategory Category { get; init; } = WikiCategory.General;
    public string? Slug { get; init; }
    public string? Summary { get; init; }
    public string? Tags { get; init; }
    public Guid? CompanyId { get; init; }
    public Guid? JobId { get; init; }
    public bool IsStarred { get; init; }
    public bool IsPinned { get; init; }
}
