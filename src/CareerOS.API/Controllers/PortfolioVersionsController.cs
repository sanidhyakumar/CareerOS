using CareerOS.Application.Features.ResumeCenter.Commands.Portfolios;
using CareerOS.Application.Features.ResumeCenter.DTOs;
using CareerOS.Application.Features.ResumeCenter.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CareerOS.API.Controllers;

[ApiController]
[Route("api/portfolio-versions")]
public class PortfolioVersionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PortfolioVersionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PortfolioVersionDto>>> GetAll()
    {
        var result = await _mediator.Send(new GetAllPortfolioVersionsQuery());
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PortfolioVersionDto>> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetPortfolioVersionByIdQuery(id));
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<PortfolioVersionDto>> Create([FromBody] CreatePortfolioVersionCommandDto dto)
    {
        var command = new CreatePortfolioVersionCommand(
            dto.Name,
            dto.Url,
            dto.TargetRole,
            dto.Description,
            dto.FeaturedProjects,
            dto.IsDefault);

        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<PortfolioVersionDto>> Update(Guid id, [FromBody] UpdatePortfolioVersionCommandDto dto)
    {
        var command = new UpdatePortfolioVersionCommand(
            id,
            dto.Name,
            dto.Url,
            dto.TargetRole,
            dto.Description,
            dto.FeaturedProjects,
            dto.IsDefault);

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeletePortfolioVersionCommand(id));
        return NoContent();
    }
}

