using CareerOS.Application.Features.Sponsorship.Commands.AddSponsorshipEvidence;
using CareerOS.Application.Features.Sponsorship.Commands.CreateSponsorshipCase;
using CareerOS.Application.Features.Sponsorship.Commands.DeleteSponsorshipCase;
using CareerOS.Application.Features.Sponsorship.Commands.RemoveSponsorshipEvidence;
using CareerOS.Application.Features.Sponsorship.Commands.UpdateSponsorshipCase;
using CareerOS.Application.Features.Sponsorship.Commands.UpdateSponsorshipEvidence;
using CareerOS.Application.Features.Sponsorship.DTOs;
using CareerOS.Application.Features.Sponsorship.Queries.GetAllSponsorshipCases;
using CareerOS.Application.Features.Sponsorship.Queries.GetSponsorshipCaseById;
using CareerOS.Application.Features.Sponsorship.Queries.GetSponsorshipStats;
using CareerOS.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CareerOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SponsorshipController : ControllerBase
{
    private readonly IMediator _mediator;

    public SponsorshipController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<SponsorshipCaseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? countryId,
        [FromQuery] VisaProgramType? programType,
        [FromQuery] SponsorshipEligibilityStatus? eligibility,
        [FromQuery] VisaAvailabilityStatus? availability,
        [FromQuery] Guid? companyId,
        [FromQuery] string? searchTerm,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllSponsorshipCasesQuery(
            CountryId: countryId,
            ProgramType: programType,
            Eligibility: eligibility,
            Availability: availability,
            CompanyId: companyId,
            SearchTerm: searchTerm), cancellationToken);

        return Ok(result);
    }

    [HttpGet("stats")]
    [ProducesResponseType(typeof(SponsorshipStatsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStats(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetSponsorshipStatsQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SponsorshipCaseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetSponsorshipCaseByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(SponsorshipCaseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateSponsorshipCaseCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(SponsorshipCaseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSponsorshipCaseCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            return BadRequest("ID in path does not match ID in body.");
        }

        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteSponsorshipCaseCommand(id), cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/evidence")]
    [ProducesResponseType(typeof(SponsorshipEvidenceDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddEvidence(Guid id, [FromBody] AddSponsorshipEvidenceCommand command, CancellationToken cancellationToken)
    {
        if (id != command.SponsorshipCaseId)
        {
            return BadRequest("Sponsorship case ID mismatch.");
        }

        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{caseId:guid}/evidence/{evidenceId:guid}")]
    [ProducesResponseType(typeof(SponsorshipEvidenceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateEvidence(Guid caseId, Guid evidenceId, [FromBody] UpdateSponsorshipEvidenceCommand command, CancellationToken cancellationToken)
    {
        if (evidenceId != command.Id)
        {
            return BadRequest("Evidence ID mismatch.");
        }

        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{caseId:guid}/evidence/{evidenceId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveEvidence(Guid caseId, Guid evidenceId, CancellationToken cancellationToken)
    {
        await _mediator.Send(new RemoveSponsorshipEvidenceCommand(evidenceId), cancellationToken);
        return NoContent();
    }
}
