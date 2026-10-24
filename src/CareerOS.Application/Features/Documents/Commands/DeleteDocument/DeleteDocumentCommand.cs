using CareerOS.Application.Common.Interfaces;
using CareerOS.Domain.Exceptions;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Documents.Commands.DeleteDocument;

public record DeleteDocumentCommand(Guid DocumentId, bool DeletePhysicalFile = true) : IRequest;

public sealed class DeleteDocumentCommandHandler : IRequestHandler<DeleteDocumentCommand>
{
    private readonly IUnitOfWork _uow;
    private readonly IFileStorageService _storageService;

    public DeleteDocumentCommandHandler(IUnitOfWork uow, IFileStorageService storageService)
    {
        _uow = uow;
        _storageService = storageService;
    }

    public async Task Handle(DeleteDocumentCommand request, CancellationToken cancellationToken)
    {
        var document = await _uow.Documents.GetByIdAsync(request.DocumentId, false, cancellationToken)
            ?? throw new NotFoundException("Document", request.DocumentId);

        var rootId = document.RootDocumentId ?? document.Id;
        var allVersions = (await _uow.Documents.GetVersionsAsync(rootId, cancellationToken)).ToList();

        if (request.DeletePhysicalFile)
        {
            foreach (var doc in allVersions)
            {
                try
                {
                    await _storageService.DeleteFileAsync(doc.StoragePath, cancellationToken);
                }
                catch
                {
                    // Physical file deletion failure shouldn't prevent DB cleanup
                }
            }
        }

        foreach (var child in allVersions.Where(v => v.Id != document.Id))
        {
            await _uow.Documents.DeleteAsync(child.Id, cancellationToken);
        }

        await _uow.Documents.DeleteAsync(document.Id, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}

