using CareerOS.Application.Features.ResumeCenter.DTOs;
using CareerOS.Application.Features.ResumeCenter.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CareerOS.API.Controllers;

[ApiController]
[Route("api/resume-analytics")]
public class ResumeAnalyticsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ResumeAnalyticsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<ResumeAnalyticsDto>> GetAnalytics()
    {
        var result = await _mediator.Send(new GetResumeAnalyticsQuery());
        return Ok(result);
    }
}

