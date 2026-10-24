using CareerOS.Application.Features.Documents.Commands.DeleteDocument;
using CareerOS.Application.Features.Documents.Commands.MoveDocument;
using CareerOS.Application.Features.Documents.Commands.UploadDocument;
using CareerOS.Application.Features.Documents.Commands.UploadDocumentVersion;
using CareerOS.Application.Features.Documents.DTOs;
using CareerOS.Application.Features.Documents.Queries;
using CareerOS.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CareerOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public DocumentsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<DocumentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? searchTerm,
        [FromQuery] Guid? countryId,
        [FromQuery] Guid? regionId,
        [FromQuery] Guid? tagId,
        [FromQuery] DocumentCategory? category,
        [FromQuery] bool? isExpired,
        [FromQuery] int? expiringWithinDays,
        [FromQuery] bool latestOnly = true,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetAllDocumentsQuery(
            searchTerm, countryId, regionId, tagId, category, isExpired, expiringWithinDays, latestOnly),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("expiring")]
    [ProducesResponseType(typeof(IEnumerable<DocumentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetExpiring(
        [FromQuery] int daysThreshold = 90,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetExpiringDocumentsQuery(daysThreshold), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(DocumentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetDocumentByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}/download")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Download(Guid id, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetDocumentFileQuery(id), cancellationToken);
        return File(response.Stream, response.ContentType, response.FileName);
    }

    [HttpGet("{id:guid}/versions")]
    [ProducesResponseType(typeof(IEnumerable<DocumentVersionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetVersions(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetDocumentVersionsQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(DocumentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Upload(
        IFormFile file,
        [FromForm] string? description,
        [FromForm] Guid? countryId,
        [FromForm] Guid? regionId,
        [FromForm] List<Guid>? tagIds,
        [FromForm] string? subFolder,
        [FromForm] DocumentCategory category = DocumentCategory.General,
        [FromForm] string? documentNumber = null,
        [FromForm] string? issuingAuthority = null,
        [FromForm] DateTime? issueDate = null,
        [FromForm] DateTime? expiryDate = null,
        CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { error = "No file was uploaded or file is empty." });
        }

        await using var stream = file.OpenReadStream();
        var command = new UploadDocumentCommand(
            stream,
            file.FileName,
            file.ContentType,
            file.Length,
            description,
            countryId,
            regionId,
            tagIds,
            subFolder,
            category,
            documentNumber,
            issuingAuthority,
            issueDate,
            expiryDate);

        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPost("{id:guid}/versions")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(DocumentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UploadVersion(
        Guid id,
        IFormFile file,
        [FromForm] string? description,
        [FromForm] DocumentCategory? category,
        [FromForm] string? documentNumber,
        [FromForm] string? issuingAuthority,
        [FromForm] DateTime? issueDate,
        [FromForm] DateTime? expiryDate,
        CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { error = "No file was uploaded or file is empty." });
        }

        await using var stream = file.OpenReadStream();
        var command = new UploadDocumentVersionCommand(
            id,
            stream,
            file.FileName,
            file.ContentType,
            file.Length,
            description,
            category,
            documentNumber,
            issuingAuthority,
            issueDate,
            expiryDate);

        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}/move")]
    [ProducesResponseType(typeof(DocumentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Move(
        Guid id,
        [FromBody] MoveDocumentRequest request,
        CancellationToken cancellationToken)
    {
        var command = new MoveDocumentCommand(id, request.TargetSubFolder);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid id,
        [FromQuery] bool deletePhysicalFile = true,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(new DeleteDocumentCommand(id, deletePhysicalFile), cancellationToken);
        return NoContent();
    }

    // Usage Tracking Endpoints
    [HttpGet("{id:guid}/usage")]
    [ProducesResponseType(typeof(IEnumerable<DocumentUsageDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsages(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetDocumentUsagesQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpPost("{id:guid}/usage")]
    [ProducesResponseType(typeof(DocumentUsageDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> AddUsage(
        Guid id,
        [FromBody] AddDocumentUsageRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddDocumentUsageCommand(
            id,
            request.EntityType,
            request.EntityId,
            request.EntityName,
            request.Purpose);

        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetUsages), new { id }, result);
    }

    [HttpDelete("usage/{usageId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteUsage(Guid usageId, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteDocumentUsageCommand(usageId), cancellationToken);
        return NoContent();
    }
}

public record MoveDocumentRequest(string TargetSubFolder);
public record AddDocumentUsageRequest(string EntityType, Guid EntityId, string? EntityName = null, string? Purpose = null);
