using CareerOS.Application.Features.Jobs.Commands.CreateJob;
using CareerOS.Application.Features.Jobs.Commands.DeleteJob;
using CareerOS.Application.Features.Jobs.Commands.UpdateJob;
using CareerOS.Application.Features.Jobs.DTOs;
using CareerOS.Application.Features.Jobs.Queries.GetAllJobs;
using CareerOS.Application.Features.Jobs.Queries.GetJobById;
using CareerOS.Application.Features.Jobs.Queries.GetJobStats;
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
    public async Task<IActionResult> GetAll(
        [FromQuery] string? searchTerm = null,
        [FromQuery] Guid? companyId = null,
        [FromQuery] JobStatus? status = null,
        [FromQuery] JobPriority? priority = null,
        [FromQuery] EmploymentType? employmentType = null,
        [FromQuery] WorkplaceType? workplaceType = null,
        [FromQuery] SeniorityLevel? seniority = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetAllJobsQuery(
            searchTerm,
            companyId,
            status,
            priority,
            employmentType,
            workplaceType,
            seniority), cancellationToken);
        return Ok(result);
    }

    [HttpGet("stats")]
    [ProducesResponseType(typeof(JobStatsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStats(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetJobStatsQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("company/{companyId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<JobDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByCompany(Guid companyId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllJobsQuery(CompanyId: companyId), cancellationToken);
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
        var command = new UpdateJobCommand(
            id,
            request.Title,
            request.Company,
            request.CompanyId,
            request.Department,
            request.Location,
            request.Url,
            request.Description,
            request.Notes,
            request.Status,
            request.Priority,
            request.EmploymentType,
            request.WorkplaceType,
            request.Seniority,
            request.MinSalary,
            request.MaxSalary,
            request.Currency ?? "USD",
            request.Salary,
            request.Equity,
            request.TechStack,
            request.Deadline,
            request.AppliedAt,
            request.InterviewAt);

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
    Guid? CompanyId = null,
    string? Department = null,
    string? Location = null,
    string? Url = null,
    string? Description = null,
    string? Notes = null,
    JobStatus Status = JobStatus.Saved,
    JobPriority Priority = JobPriority.Medium,
    EmploymentType EmploymentType = EmploymentType.FullTime,
    WorkplaceType WorkplaceType = WorkplaceType.Remote,
    SeniorityLevel Seniority = SeniorityLevel.Senior,
    decimal? MinSalary = null,
    decimal? MaxSalary = null,
    string Currency = "USD",
    string? Salary = null,
    string? Equity = null,
    string? TechStack = null,
    DateTime? Deadline = null,
    DateTime? AppliedAt = null,
    DateTime? InterviewAt = null
);
