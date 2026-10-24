using CareerOS.Application.Features.Documents.Commands.DeleteDocument;
using CareerOS.Application.Features.Documents.Commands.MoveDocument;
using CareerOS.Application.Features.Documents.Commands.UploadDocument;
using CareerOS.Application.Features.Documents.Commands.UploadDocumentVersion;
using CareerOS.Application.Features.Documents.DTOs;
using CareerOS.Application.Features.Documents.Queries;
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
        [FromQuery] bool latestOnly = true,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetAllDocumentsQuery(searchTerm, countryId, regionId, tagId, latestOnly), cancellationToken);
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
        CancellationToken cancellationToken)
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
            subFolder);

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
        CancellationToken cancellationToken)
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
            description);

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
}

public record MoveDocumentRequest(string TargetSubFolder);

