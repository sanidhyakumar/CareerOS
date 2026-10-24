using AutoMapper;
using CareerOS.Application.Common.Interfaces;
using CareerOS.Application.Features.Documents.DTOs;
using CareerOS.Domain.Entities;
using CareerOS.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace CareerOS.Application.Features.Documents.Commands.UploadDocument;

public record UploadDocumentCommand(
    Stream FileStream,
    string OriginalFileName,
    string ContentType,
    long FileSizeBytes,
    string? Description = null,
    Guid? CountryId = null,
    Guid? RegionId = null,
    List<Guid>? TagIds = null,
    string? SubFolder = null
) : IRequest<DocumentDto>;

public class UploadDocumentCommandValidator : AbstractValidator<UploadDocumentCommand>
{
    public UploadDocumentCommandValidator()
    {
        RuleFor(x => x.FileStream).NotNull().WithMessage("File content is required.");
        RuleFor(x => x.OriginalFileName).NotEmpty().MaximumLength(255);
        RuleFor(x => x.ContentType).NotEmpty().MaximumLength(150);
        RuleFor(x => x.FileSizeBytes).GreaterThan(0).WithMessage("File cannot be empty.");
    }
}

public sealed class UploadDocumentCommandHandler : IRequestHandler<UploadDocumentCommand, DocumentDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IFileStorageService _storageService;
    private readonly IMetadataEngine _metadataEngine;
    private readonly IMapper _mapper;

    public UploadDocumentCommandHandler(
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

    public async Task<DocumentDto> Handle(UploadDocumentCommand request, CancellationToken cancellationToken)
    {
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

        // 3. Determine unique storage path
        var folder = string.IsNullOrWhiteSpace(request.SubFolder)
            ? $"documents/{DateTime.UtcNow:yyyy/MM}"
            : request.SubFolder.Trim('/');

        var storedFileName = $"{Guid.NewGuid()}{metadata.FileExtension}";
        var relativePath = $"{folder}/{storedFileName}";

        // 4. Save file to storage
        var finalStoragePath = await _storageService.SaveFileAsync(request.FileStream, relativePath, cancellationToken);

        // 5. Create Document entity
        var document = Document.Create(
            storedFileName,
            metadata.OriginalFileName,
            metadata.FileExtension,
            metadata.ContentType,
            metadata.FileSizeBytes,
            finalStoragePath,
            metadata.Sha256Hash,
            request.Description,
            request.CountryId,
            request.RegionId);

        // 6. Attach tags if provided
        if (request.TagIds != null && request.TagIds.Count > 0)
        {
            var validTags = await _uow.Tags.GetByIdsAsync(request.TagIds, cancellationToken);
            foreach (var tag in validTags)
            {
                document.DocumentTags.Add(new DocumentTag
                {
                    Document = document,
                    Tag = tag,
                    TagId = tag.Id
                });
            }
        }

        await _uow.Documents.AddAsync(document, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        // Reload to include relations for DTO
        var created = await _uow.Documents.GetByIdAsync(document.Id, includeDetails: true, cancellationToken);
        return _mapper.Map<DocumentDto>(created ?? document);
    }
}

