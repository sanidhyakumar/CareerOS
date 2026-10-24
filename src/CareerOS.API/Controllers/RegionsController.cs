using CareerOS.Application.Features.Countries.Commands.CreateRegion;
using CareerOS.Application.Features.Countries.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CareerOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RegionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public RegionsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<RegionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByCountry([FromQuery] Guid countryId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetRegionsByCountryQuery(countryId), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(RegionDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateRegionCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}

