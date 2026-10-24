using CareerOS.Application.Features.Jobs.Commands.CreateJob;
using CareerOS.Application.Features.Jobs.Commands.DeleteJob;
using CareerOS.Application.Features.Jobs.Commands.UpdateJob;
using CareerOS.Application.Features.Jobs.DTOs;
using CareerOS.Application.Features.Jobs.Queries.GetAllJobs;
using CareerOS.Application.Features.Jobs.Queries.GetJobById;
using CareerOS.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CareerOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
    private readonly IMediator _mediator;

    public JobsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<JobDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllJobsQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(JobDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetJobByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(JobDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateJobCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(JobDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateJobRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateJobCommand(id, request.Title, request.Company, request.Location,
            request.Url, request.Description, request.Notes, request.Status, request.Salary,
            request.AppliedAt, request.InterviewAt);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteJobCommand(id), cancellationToken);
        return NoContent();
    }
}

public record UpdateJobRequest(
    string Title,
    string Company,
    string? Location,
    string? Url,
    string? Description,
    string? Notes,
    JobStatus Status,
    string? Salary,
    DateTime? AppliedAt,
    DateTime? InterviewAt
);

