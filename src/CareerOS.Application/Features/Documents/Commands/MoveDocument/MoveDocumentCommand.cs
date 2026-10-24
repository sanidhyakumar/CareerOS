using AutoMapper;
using CareerOS.Application.Common.Interfaces;
using CareerOS.Application.Features.Documents.DTOs;
using CareerOS.Domain.Exceptions;
using CareerOS.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace CareerOS.Application.Features.Documents.Commands.MoveDocument;

public record MoveDocumentCommand(Guid DocumentId, string TargetSubFolder) : IRequest<DocumentDto>;

public class MoveDocumentCommandValidator : AbstractValidator<MoveDocumentCommand>
{
    public MoveDocumentCommandValidator()
    {
        RuleFor(x => x.DocumentId).NotEmpty();
        RuleFor(x => x.TargetSubFolder).NotEmpty().MaximumLength(200);
    }
}

public sealed class MoveDocumentCommandHandler : IRequestHandler<MoveDocumentCommand, DocumentDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IFileStorageService _storageService;
    private readonly IMapper _mapper;

    public MoveDocumentCommandHandler(IUnitOfWork uow, IFileStorageService storageService, IMapper mapper)
    {
        _uow = uow;
        _storageService = storageService;
        _mapper = mapper;
    }

    public async Task<DocumentDto> Handle(MoveDocumentCommand request, CancellationToken cancellationToken)
    {
        var document = await _uow.Documents.GetByIdAsync(request.DocumentId, includeDetails: true, cancellationToken)
            ?? throw new NotFoundException("Document", request.DocumentId);

        var targetFolder = request.TargetSubFolder.Trim().Trim('/');
        var newRelativePath = $"{targetFolder}/{document.FileName}";

        if (document.StoragePath != newRelativePath)
        {
            await _storageService.MoveFileAsync(document.StoragePath, newRelativePath, cancellationToken);
            document.Move(newRelativePath);

            await _uow.Documents.UpdateAsync(document, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);
        }

        return _mapper.Map<DocumentDto>(document);
    }
}

