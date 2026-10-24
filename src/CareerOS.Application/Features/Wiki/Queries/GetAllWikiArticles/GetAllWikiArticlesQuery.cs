using AutoMapper;
using CareerOS.Application.Features.Wiki.DTOs;
using CareerOS.Domain.Enums;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Wiki.Queries.GetAllWikiArticles;

public record GetAllWikiArticlesQuery(
    string? SearchTerm = null,
    WikiCategory? Category = null,
    string? Tag = null,
    Guid? CompanyId = null,
    bool? IsStarred = null) : IRequest<List<WikiArticleDto>>;

public class GetAllWikiArticlesQueryHandler : IRequestHandler<GetAllWikiArticlesQuery, List<WikiArticleDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetAllWikiArticlesQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<List<WikiArticleDto>> Handle(GetAllWikiArticlesQuery request, CancellationToken cancellationToken)
    {
        var articles = await _uow.Wiki.GetAllAsync(
            searchTerm: request.SearchTerm,
            category: request.Category,
            tag: request.Tag,
            companyId: request.CompanyId,
            isStarred: request.IsStarred,
            cancellationToken: cancellationToken);

        return _mapper.Map<List<WikiArticleDto>>(articles);
    }
}

