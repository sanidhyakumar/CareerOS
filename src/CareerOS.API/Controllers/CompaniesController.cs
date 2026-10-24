using CareerOS.Application.Features.Companies.Commands;
using CareerOS.Application.Features.Companies.DTOs;
using CareerOS.Application.Features.Companies.Queries;
using CareerOS.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CareerOS.API.Controllers;

[ApiController]
[Route("api/companies")]
public class CompaniesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CompaniesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CompanyDto>>> GetAll(
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? industry = null,
        [FromQuery] SponsorshipStatus? sponsorship = null,
        [FromQuery] RelocationStatus? relocation = null,
        [FromQuery] int? minRating = null,
        [FromQuery] bool? isTarget = null)
    {
        var result = await _mediator.Send(new GetAllCompaniesQuery(
            searchTerm,
            industry,
            sponsorship,
            relocation,
            minRating,
            isTarget));

        return Ok(result);
    }

    [HttpGet("stats")]
    public async Task<ActionResult<CompanyStatsDto>> GetStats()
    {
        var result = await _mediator.Send(new GetCompanyStatsQuery());
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CompanyDto>> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetCompanyByIdQuery(id));
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<CompanyDto>> Create([FromBody] CreateCompanyCommandDto dto)
    {
        var command = new CreateCompanyCommand(
            dto.Name,
            dto.Description,
            dto.Headquarters,
            dto.Size,
            dto.Stage,
            dto.FoundedYear,
            dto.LogoUrl,
            dto.WebsiteUrl,
            dto.CareersUrl,
            dto.TechBlogUrl,
            dto.LinkedInUrl,
            dto.GlassdoorUrl,
            dto.GlassdoorRating,
            dto.PersonalRating,
            dto.CultureRating,
            dto.WorkLifeBalanceRating,
            dto.SponsorshipStatus,
            dto.SponsoredVisas,
            dto.SponsorshipNotes,
            dto.RelocationStatus,
            dto.RelocationNotes,
            dto.Industry,
            dto.Sector,
            dto.Tags,
            dto.Notes,
            dto.Pros,
            dto.Cons,
            dto.IsTarget);

        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CompanyDto>> Update(Guid id, [FromBody] UpdateCompanyCommandDto dto)
    {
        var command = new UpdateCompanyCommand(
            id,
            dto.Name,
            dto.Description,
            dto.Headquarters,
            dto.Size,
            dto.Stage,
            dto.FoundedYear,
            dto.LogoUrl,
            dto.WebsiteUrl,
            dto.CareersUrl,
            dto.TechBlogUrl,
            dto.LinkedInUrl,
            dto.GlassdoorUrl,
            dto.GlassdoorRating,
            dto.PersonalRating,
            dto.CultureRating,
            dto.WorkLifeBalanceRating,
            dto.SponsorshipStatus,
            dto.SponsoredVisas,
            dto.SponsorshipNotes,
            dto.RelocationStatus,
            dto.RelocationNotes,
            dto.Industry,
            dto.Sector,
            dto.Tags,
            dto.Notes,
            dto.Pros,
            dto.Cons,
            dto.IsTarget);

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteCompanyCommand(id));
        return NoContent();
    }
}

