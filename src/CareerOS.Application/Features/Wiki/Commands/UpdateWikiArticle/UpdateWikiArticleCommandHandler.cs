using AutoMapper;
using CareerOS.Domain.Exceptions;
using CareerOS.Application.Features.Wiki.DTOs;
using CareerOS.Domain.Entities;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Wiki.Commands.UpdateWikiArticle;

public class UpdateWikiArticleCommandHandler : IRequestHandler<UpdateWikiArticleCommand, WikiArticleDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public UpdateWikiArticleCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<WikiArticleDto> Handle(UpdateWikiArticleCommand request, CancellationToken cancellationToken)
    {
        var article = await _uow.Wiki.GetByIdAsync(request.Id, includeLinks: false, cancellationToken: cancellationToken);
        if (article == null)
        {
            throw new NotFoundException(nameof(WikiArticle), request.Id);
        }

        article.Update(
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

        await _uow.Wiki.UpdateAsync(article, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        await _uow.Wiki.SyncBacklinksAsync(article.Id, article.Content, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        var loaded = await _uow.Wiki.GetByIdAsync(article.Id, includeLinks: true, cancellationToken: cancellationToken);
        return _mapper.Map<WikiArticleDto>(loaded ?? article);
    }
}
