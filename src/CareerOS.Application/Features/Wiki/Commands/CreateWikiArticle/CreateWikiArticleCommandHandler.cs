using AutoMapper;
using CareerOS.Application.Features.Wiki.DTOs;
using CareerOS.Domain.Entities;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Wiki.Commands.CreateWikiArticle;

public class CreateWikiArticleCommandHandler : IRequestHandler<CreateWikiArticleCommand, WikiArticleDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CreateWikiArticleCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<WikiArticleDto> Handle(CreateWikiArticleCommand request, CancellationToken cancellationToken)
    {
        var article = WikiArticle.Create(
            title: request.Title,
            content: request.Content,
            category: request.Category,
            slug: request.Slug,
            summary: request.Summary,
            tags: request.Tags,
            companyId: request.CompanyId,
            jobId: request.JobId,
            isStarred: request.IsStarred,
            isPinned: request.IsPinned);

        await _uow.Wiki.AddAsync(article, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        // Sync outlinks / backlinks extracted from [[slug]] or [[slug|Link Text]]
        await _uow.Wiki.SyncBacklinksAsync(article.Id, article.Content, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        var loaded = await _uow.Wiki.GetByIdAsync(article.Id, includeLinks: true, cancellationToken: cancellationToken);
        return _mapper.Map<WikiArticleDto>(loaded ?? article);
    }
}
