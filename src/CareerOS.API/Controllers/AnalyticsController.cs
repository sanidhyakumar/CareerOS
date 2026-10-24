using CareerOS.Application.Features.Analytics.DTOs;
using CareerOS.Application.Features.Analytics.Queries.GetCareerAnalytics;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CareerOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnalyticsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AnalyticsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieve comprehensive career intelligence dashboard analytics
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(CareerAnalyticsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CareerAnalyticsDto>> GetAnalytics(
        [FromQuery] int? days = null,
        [FromQuery] Guid? profileId = null)
    {
        var result = await _mediator.Send(new GetCareerAnalyticsQuery(days, profileId));
        return Ok(result);
    }

    /// <summary>
    /// Retrieve application conversion funnel stages
    /// </summary>
    [HttpGet("funnel")]
    [ProducesResponseType(typeof(List<ApplicationFunnelStageDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ApplicationFunnelStageDto>>> GetFunnel(
        [FromQuery] int? days = null,
        [FromQuery] Guid? profileId = null)
    {
        var result = await _mediator.Send(new GetCareerAnalyticsQuery(days, profileId));
        return Ok(result.Funnel);
    }

    /// <summary>
    /// Retrieve tailored resume version effectiveness statistics
    /// </summary>
    [HttpGet("resumes")]
    [ProducesResponseType(typeof(List<ResumeEffectivenessDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ResumeEffectivenessDto>>> GetResumeEffectiveness(
        [FromQuery] int? days = null,
        [FromQuery] Guid? profileId = null)
    {
        var result = await _mediator.Send(new GetCareerAnalyticsQuery(days, profileId));
        return Ok(result.ResumeEffectiveness);
    }

    /// <summary>
    /// Retrieve rejection stage distribution and feedback notes
    /// </summary>
    [HttpGet("rejections")]
    [ProducesResponseType(typeof(RejectionAnalyticsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<RejectionAnalyticsDto>> GetRejections(
        [FromQuery] int? days = null,
        [FromQuery] Guid? profileId = null)
    {
        var result = await _mediator.Send(new GetCareerAnalyticsQuery(days, profileId));
        return Ok(result.RejectionAnalytics);
    }
}

