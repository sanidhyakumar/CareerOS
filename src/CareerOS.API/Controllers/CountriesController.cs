using CareerOS.Application.Features.Countries.Commands.CreateCountry;
using CareerOS.Application.Features.Countries.Commands.CreateRegion;
using CareerOS.Application.Features.Countries.DTOs;
using CareerOS.Application.Features.Countries.Queries.GetAllCountries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CareerOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CountriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CountriesController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CountryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] bool activeOnly = false, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetAllCountriesQuery(activeOnly), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CountryDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateCountryCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}/regions")]
    [ProducesResponseType(typeof(IEnumerable<RegionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRegions(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetRegionsByCountryQuery(id), cancellationToken);
        return Ok(result);
    }
}

