using AutoMapper;
using CareerOS.Domain.Exceptions;
using CareerOS.Application.Features.Wiki.DTOs;
using CareerOS.Domain.Entities;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Wiki.Commands.ToggleWikiStar;

public record ToggleWikiStarCommand(Guid Id) : IRequest<WikiArticleDto>;

public class ToggleWikiStarCommandHandler : IRequestHandler<ToggleWikiStarCommand, WikiArticleDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public ToggleWikiStarCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<WikiArticleDto> Handle(ToggleWikiStarCommand request, CancellationToken cancellationToken)
    {
        var article = await _uow.Wiki.GetByIdAsync(request.Id, includeLinks: true, cancellationToken: cancellationToken);
        if (article == null)
        {
            throw new NotFoundException(nameof(WikiArticle), request.Id);
        }

        article.IsStarred = !article.IsStarred;
        article.UpdatedAt = DateTime.UtcNow;

        await _uow.Wiki.UpdateAsync(article, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return _mapper.Map<WikiArticleDto>(article);
    }
}
