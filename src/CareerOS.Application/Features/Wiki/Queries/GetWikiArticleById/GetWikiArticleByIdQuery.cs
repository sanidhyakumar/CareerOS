using AutoMapper;
using CareerOS.Domain.Exceptions;
using CareerOS.Application.Features.Wiki.DTOs;
using CareerOS.Domain.Entities;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Wiki.Queries.GetWikiArticleById;

public record GetWikiArticleByIdQuery(Guid Id, bool IncrementView = true) : IRequest<WikiArticleDto>;

public class GetWikiArticleByIdQueryHandler : IRequestHandler<GetWikiArticleByIdQuery, WikiArticleDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetWikiArticleByIdQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<WikiArticleDto> Handle(GetWikiArticleByIdQuery request, CancellationToken cancellationToken)
    {
        var article = await _uow.Wiki.GetByIdAsync(request.Id, includeLinks: true, cancellationToken: cancellationToken);
        if (article == null)
        {
            throw new NotFoundException(nameof(WikiArticle), request.Id);
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
