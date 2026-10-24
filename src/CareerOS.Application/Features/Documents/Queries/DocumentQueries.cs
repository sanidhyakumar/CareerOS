using AutoMapper;
using CareerOS.Application.Common.Interfaces;
using CareerOS.Application.Features.Documents.DTOs;
using CareerOS.Domain.Exceptions;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Documents.Queries;

public record GetAllDocumentsQuery(
    string? SearchTerm = null,
    Guid? CountryId = null,
    Guid? RegionId = null,
    Guid? TagId = null,
    bool LatestOnly = true
) : IRequest<IEnumerable<DocumentDto>>;

public sealed class GetAllDocumentsQueryHandler : IRequestHandler<GetAllDocumentsQuery, IEnumerable<DocumentDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetAllDocumentsQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<IEnumerable<DocumentDto>> Handle(GetAllDocumentsQuery request, CancellationToken cancellationToken)
    {
        var documents = await _uow.Documents.SearchAsync(
            request.SearchTerm,
            request.CountryId,
            request.RegionId,
            request.TagId,
            cancellationToken);

        if (request.LatestOnly)
        {
            documents = documents.Where(d => d.IsLatestVersion);
        }

        return _mapper.Map<IEnumerable<DocumentDto>>(documents);
    }
}

public record GetDocumentByIdQuery(Guid Id) : IRequest<DocumentDto>;

public sealed class GetDocumentByIdQueryHandler : IRequestHandler<GetDocumentByIdQuery, DocumentDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetDocumentByIdQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<DocumentDto> Handle(GetDocumentByIdQuery request, CancellationToken cancellationToken)
    {
        var document = await _uow.Documents.GetByIdAsync(request.Id, includeDetails: true, cancellationToken)
            ?? throw new NotFoundException("Document", request.Id);

        return _mapper.Map<DocumentDto>(document);
    }
}

public record GetDocumentFileQuery(Guid Id) : IRequest<DocumentFileResponse>;

public record DocumentFileResponse(Stream Stream, string ContentType, string FileName);

public sealed class GetDocumentFileQueryHandler : IRequestHandler<GetDocumentFileQuery, DocumentFileResponse>
{
    private readonly IUnitOfWork _uow;
    private readonly IFileStorageService _storageService;

    public GetDocumentFileQueryHandler(IUnitOfWork uow, IFileStorageService storageService)
    {
        _uow = uow;
        _storageService = storageService;
    }

    public async Task<DocumentFileResponse> Handle(GetDocumentFileQuery request, CancellationToken cancellationToken)
    {
        var document = await _uow.Documents.GetByIdAsync(request.Id, false, cancellationToken)
            ?? throw new NotFoundException("Document", request.Id);

        var stream = await _storageService.ReadFileStreamAsync(document.StoragePath, cancellationToken);
        return new DocumentFileResponse(stream, document.ContentType, document.OriginalFileName);
    }
}

public record GetDocumentVersionsQuery(Guid DocumentId) : IRequest<IEnumerable<DocumentVersionDto>>;

public sealed class GetDocumentVersionsQueryHandler : IRequestHandler<GetDocumentVersionsQuery, IEnumerable<DocumentVersionDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetDocumentVersionsQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<IEnumerable<DocumentVersionDto>> Handle(GetDocumentVersionsQuery request, CancellationToken cancellationToken)
    {
        var versions = await _uow.Documents.GetVersionsAsync(request.DocumentId, cancellationToken);
        return _mapper.Map<IEnumerable<DocumentVersionDto>>(versions);
    }
}

