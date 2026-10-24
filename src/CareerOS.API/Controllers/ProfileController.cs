using CareerOS.Application.Features.Profile.Commands.Addresses;
using CareerOS.Application.Features.Profile.Commands.Citizenships;
using CareerOS.Application.Features.Profile.Commands.SocialLinks;
using CareerOS.Application.Features.Profile.Commands.UpdatePersonalInfo;
using CareerOS.Application.Features.Profile.Commands.WorkAuthorizations;
using CareerOS.Application.Features.Profile.DTOs;
using CareerOS.Application.Features.Profile.Queries.GetProfile;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CareerOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProfileController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProfileController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetProfileQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPut("personal-info")]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePersonalInfo(
        [FromBody] UpdatePersonalInfoCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPost("citizenships")]
    [ProducesResponseType(typeof(ProfileCitizenshipDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddCitizenship(
        [FromBody] AddCitizenshipCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetProfile), null, result);
    }

    [HttpDelete("citizenships/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteCitizenship(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteCitizenshipCommand(id), cancellationToken);
        return NoContent();
    }

    [HttpPut("citizenships/{id:guid}/primary")]
    [ProducesResponseType(typeof(ProfileCitizenshipDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> SetPrimaryCitizenship(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new SetPrimaryCitizenshipCommand(id), cancellationToken);
        return Ok(result);
    }

    [HttpPost("work-authorizations")]
    [ProducesResponseType(typeof(WorkAuthorizationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddWorkAuthorization(
        [FromBody] AddWorkAuthorizationCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetProfile), null, result);
    }

    [HttpPut("work-authorizations/{id:guid}")]
    [ProducesResponseType(typeof(WorkAuthorizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateWorkAuthorization(
        Guid id,
        [FromBody] UpdateWorkAuthorizationRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateWorkAuthorizationCommand(
            id,
            request.CountryId,
            request.AuthorizationType,
            request.VisaType,
            request.RequiresSponsorship,
            request.ValidFrom,
            request.ValidTo,
            request.DocumentId,
            request.Notes);

        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("work-authorizations/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteWorkAuthorization(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteWorkAuthorizationCommand(id), cancellationToken);
        return NoContent();
    }

    [HttpPost("social-links")]
    [ProducesResponseType(typeof(SocialLinkDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddSocialLink(
        [FromBody] AddSocialLinkCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetProfile), null, result);
    }

    [HttpPut("social-links/{id:guid}")]
    [ProducesResponseType(typeof(SocialLinkDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateSocialLink(
        Guid id,
        [FromBody] UpdateSocialLinkRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateSocialLinkCommand(
            id,
            request.Platform,
            request.Url,
            request.DisplayLabel,
            request.DisplayOrder);

        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("social-links/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteSocialLink(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteSocialLinkCommand(id), cancellationToken);
        return NoContent();
    }

    [HttpPost("addresses")]
    [ProducesResponseType(typeof(AddressDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddAddress(
        [FromBody] AddAddressCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetProfile), null, result);
    }

    [HttpPut("addresses/{id:guid}")]
    [ProducesResponseType(typeof(AddressDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateAddress(
        Guid id,
        [FromBody] UpdateAddressRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateAddressCommand(
            id,
            request.AddressType,
            request.Street1,
            request.Street2,
            request.City,
            request.StateProvince,
            request.RegionId,
            request.PostalCode,
            request.CountryId,
            request.IsPrimary,
            request.WillingToRelocate,
            request.OpenToRemote);

        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("addresses/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAddress(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteAddressCommand(id), cancellationToken);
        return NoContent();
    }

    [HttpPut("addresses/{id:guid}/primary")]
    [ProducesResponseType(typeof(AddressDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> SetPrimaryAddress(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new SetPrimaryAddressCommand(id), cancellationToken);
        return Ok(result);
    }
}

public record UpdateWorkAuthorizationRequest(
    Guid CountryId,
    CareerOS.Domain.Enums.WorkAuthorizationType AuthorizationType,
    string? VisaType = null,
    bool RequiresSponsorship = false,
    DateTime? ValidFrom = null,
    DateTime? ValidTo = null,
    Guid? DocumentId = null,
    string? Notes = null);

public record UpdateSocialLinkRequest(
    CareerOS.Domain.Enums.SocialPlatform Platform,
    string Url,
    string? DisplayLabel = null,
    int DisplayOrder = 0);

public record UpdateAddressRequest(
    CareerOS.Domain.Enums.AddressType AddressType,
    string Street1,
    string? Street2,
    string City,
    string StateProvince,
    Guid? RegionId,
    string PostalCode,
    Guid CountryId,
    bool IsPrimary,
    bool WillingToRelocate,
    bool OpenToRemote);

