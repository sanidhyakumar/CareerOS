using CareerOS.Application.Common.Interfaces;
using CareerOS.Application.Features.CareerProfiles.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CareerOS.API.Controllers;

[ApiController]
[Route("api/profiles")]
public class CareerProfilesController : ControllerBase
{
    private readonly ICareerProfileService _profileService;
    private readonly ILogger<CareerProfilesController> _logger;

    public CareerProfilesController(
        ICareerProfileService profileService,
        ILogger<CareerProfilesController> logger)
    {
        _profileService = profileService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<CareerProfileDto>>> GetAll(CancellationToken cancellationToken)
    {
        var profiles = await _profileService.GetAllProfilesAsync(cancellationToken);
        return Ok(profiles);
    }

    [HttpGet("active")]
    public async Task<ActionResult<CareerProfileDto>> GetActive(CancellationToken cancellationToken)
    {
        var active = await _profileService.GetActiveProfileAsync(cancellationToken);
        return Ok(active);
    }

    [HttpPost("active/{id:guid}")]
    public async Task<IActionResult> SetActive(Guid id, CancellationToken cancellationToken)
    {
        await _profileService.SetActiveProfileAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpGet("family-dashboard")]
    public async Task<ActionResult<FamilyDashboardDto>> GetFamilyDashboard(CancellationToken cancellationToken)
    {
        var dashboard = await _profileService.GetFamilyDashboardAsync(cancellationToken);
        return Ok(dashboard);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CareerProfileDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var profile = await _profileService.GetProfileByIdAsync(id, cancellationToken);
        if (profile == null) return NotFound();
        return Ok(profile);
    }

    [HttpGet("slug/{slug}")]
    public async Task<ActionResult<CareerProfileDto>> GetBySlug(string slug, CancellationToken cancellationToken)
    {
        var profile = await _profileService.GetProfileBySlugAsync(slug, cancellationToken);
        if (profile == null) return NotFound();
        return Ok(profile);
    }

    [HttpGet("{id:guid}/analytics")]
    public async Task<ActionResult<ProfileAnalyticsDto>> GetAnalytics(Guid id, CancellationToken cancellationToken)
    {
        var analytics = await _profileService.GetProfileAnalyticsAsync(id, cancellationToken);
        return Ok(analytics);
    }

    [HttpPost]
    public async Task<ActionResult<CareerProfileDto>> Create([FromBody] CreateCareerProfileDto dto, CancellationToken cancellationToken)
    {
        var created = await _profileService.CreateProfileAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CareerProfileDto>> Update(Guid id, [FromBody] UpdateCareerProfileDto dto, CancellationToken cancellationToken)
    {
        var updated = await _profileService.UpdateProfileAsync(id, dto, cancellationToken);
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _profileService.DeleteProfileAsync(id, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

