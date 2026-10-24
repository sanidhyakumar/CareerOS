using AutoMapper;
using CareerOS.Domain.Exceptions;
using CareerOS.Application.Features.Wiki.DTOs;
using CareerOS.Domain.Entities;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Wiki.Queries.GetWikiArticleBySlug;

public record GetWikiArticleBySlugQuery(string Slug, bool IncrementView = true) : IRequest<WikiArticleDto>;

public class GetWikiArticleBySlugQueryHandler : IRequestHandler<GetWikiArticleBySlugQuery, WikiArticleDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetWikiArticleBySlugQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<WikiArticleDto> Handle(GetWikiArticleBySlugQuery request, CancellationToken cancellationToken)
    {
        var article = await _uow.Wiki.GetBySlugAsync(request.Slug, includeLinks: true, cancellationToken: cancellationToken);
        if (article == null)
        {
            throw new NotFoundException(nameof(WikiArticle), request.Slug);
        }

        if (request.IncrementView)
        {
            article.ViewCount++;
            await _uow.Wiki.UpdateAsync(article, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);
        }

        return _mapper.Map<WikiArticleDto>(article);
    }
}
