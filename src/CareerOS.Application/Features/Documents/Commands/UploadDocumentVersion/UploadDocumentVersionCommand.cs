using AutoMapper;
using CareerOS.Application.Common.Interfaces;
using CareerOS.Application.Features.Documents.DTOs;
using CareerOS.Domain.Entities;
using CareerOS.Domain.Exceptions;
using CareerOS.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace CareerOS.Application.Features.Documents.Commands.UploadDocumentVersion;

public record UploadDocumentVersionCommand(
    Guid DocumentId,
    Stream FileStream,
    string OriginalFileName,
    string ContentType,
    long FileSizeBytes,
    string? Description = null
) : IRequest<DocumentDto>;

public class UploadDocumentVersionCommandValidator : AbstractValidator<UploadDocumentVersionCommand>
{
    public UploadDocumentVersionCommandValidator()
    {
        RuleFor(x => x.DocumentId).NotEmpty();
        RuleFor(x => x.FileStream).NotNull();
        RuleFor(x => x.OriginalFileName).NotEmpty();
        RuleFor(x => x.FileSizeBytes).GreaterThan(0);
    }
}

public sealed class UploadDocumentVersionCommandHandler : IRequestHandler<UploadDocumentVersionCommand, DocumentDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IFileStorageService _storageService;
    private readonly IMetadataEngine _metadataEngine;
    private readonly IMapper _mapper;

    public UploadDocumentVersionCommandHandler(
        IUnitOfWork uow,
        IFileStorageService storageService,
        IMetadataEngine metadataEngine,
        IMapper mapper)
    {
        _uow = uow;
        _storageService = storageService;
        _metadataEngine = metadataEngine;
        _mapper = mapper;
    }

    public async Task<DocumentDto> Handle(UploadDocumentVersionCommand request, CancellationToken cancellationToken)
    {
        var existingDoc = await _uow.Documents.GetByIdAsync(request.DocumentId, includeDetails: true, cancellationToken)
            ?? throw new NotFoundException("Document", request.DocumentId);

        // 1. Calculate SHA256
        var sha256 = await _metadataEngine.ComputeSha256Async(request.FileStream, cancellationToken);
        if (request.FileStream.CanSeek)
        {
            request.FileStream.Position = 0;
        }

        // 2. Extract metadata
        var metadata = _metadataEngine.ExtractMetadata(
            request.OriginalFileName,
            request.FileSizeBytes,
            request.ContentType,
            sha256);

        // 3. Storage location in same parent directory
        var existingDir = Path.GetDirectoryName(existingDoc.StoragePath)?.Replace('\\', '/') ?? "documents";
        var storedFileName = $"{Guid.NewGuid()}{metadata.FileExtension}";
        var relativePath = $"{existingDir}/{storedFileName}";

        // 4. Save file
        var finalStoragePath = await _storageService.SaveFileAsync(request.FileStream, relativePath, cancellationToken);

        // 5. Create new version entity
        var newVersion = existingDoc.CreateNewVersion(
            storedFileName,
            metadata.OriginalFileName,
            metadata.FileExtension,
            metadata.ContentType,
            metadata.FileSizeBytes,
            finalStoragePath,
            metadata.Sha256Hash,
            request.Description);

        // Copy tags from existing document
        foreach (var dt in existingDoc.DocumentTags)
        {
            newVersion.DocumentTags.Add(new DocumentTag
            {
                Document = newVersion,
                TagId = dt.TagId
            });
        }

        await _uow.Documents.UpdateAsync(existingDoc, cancellationToken);
        await _uow.Documents.AddAsync(newVersion, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        var created = await _uow.Documents.GetByIdAsync(newVersion.Id, includeDetails: true, cancellationToken);
        return _mapper.Map<DocumentDto>(created ?? newVersion);
    }
}

