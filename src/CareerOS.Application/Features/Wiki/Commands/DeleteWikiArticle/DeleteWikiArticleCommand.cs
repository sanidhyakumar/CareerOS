using CareerOS.Domain.Exceptions;
using CareerOS.Domain.Entities;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Wiki.Commands.DeleteWikiArticle;

public record DeleteWikiArticleCommand(Guid Id) : IRequest;

public class DeleteWikiArticleCommandHandler : IRequestHandler<DeleteWikiArticleCommand>
{
    private readonly IUnitOfWork _uow;

    public DeleteWikiArticleCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task Handle(DeleteWikiArticleCommand request, CancellationToken cancellationToken)
    {
        var article = await _uow.Wiki.GetByIdAsync(request.Id, includeLinks: false, cancellationToken: cancellationToken);
        if (article == null)
        {
            throw new NotFoundException(nameof(WikiArticle), request.Id);
        }

        await _uow.Wiki.DeleteAsync(article, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}
