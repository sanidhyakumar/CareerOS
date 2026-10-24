using CareerOS.Application.Features.Security.Commands.ConfigureMasterPassword;
using CareerOS.Application.Features.Security.Commands.CreateCredential;
using CareerOS.Application.Features.Security.Commands.DeleteCredential;
using CareerOS.Application.Features.Security.Commands.LockSecurityVault;
using CareerOS.Application.Features.Security.Commands.UnlockSecurityVault;
using CareerOS.Application.Features.Security.Commands.UpdateCredential;
using CareerOS.Application.Features.Security.DTOs;
using CareerOS.Application.Features.Security.Queries.GetCredentials;
using CareerOS.Application.Features.Security.Queries.GetSecurityStatus;
using CareerOS.Application.Features.Security.Queries.RevealCredential;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CareerOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SecurityController : ControllerBase
{
    private readonly IMediator _mediator;

    public SecurityController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieve current vault status (whether master password is set and whether vault is unlocked)
    /// </summary>
    [HttpGet("status")]
    [ProducesResponseType(typeof(SecurityStatusDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<SecurityStatusDto>> GetStatus()
    {
        var result = await _mediator.Send(new GetSecurityStatusQuery());
        return Ok(result);
    }

    /// <summary>
    /// Initialize or update the master passphrase (AES-256 + PBKDF2 with SHA-256)
    /// </summary>
    [HttpPost("setup")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> SetupMasterPassword([FromBody] ConfigureMasterPasswordCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Unlock the security vault with master password to derive the active AES-256 key
    /// </summary>
    [HttpPost("unlock")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> UnlockVault([FromBody] UnlockSecurityVaultCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result)
        {
            return BadRequest(new { message = "Invalid master password. Please verify and try again." });
        }
        return Ok(new { success = true, message = "Security vault unlocked." });
    }

    /// <summary>
    /// Lock the security vault and zero-out active encryption keys in memory
    /// </summary>
    [HttpPost("lock")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> LockVault()
    {
        await _mediator.Send(new LockSecurityVaultCommand());
        return Ok(new { success = true, message = "Security vault locked." });
    }

    /// <summary>
    /// List stored portal credentials (masked passwords)
    /// </summary>
    [HttpGet("credentials")]
    [ProducesResponseType(typeof(List<CredentialDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CredentialDto>>> GetCredentials([FromQuery] Guid? companyId = null)
    {
        var result = await _mediator.Send(new GetCredentialsQuery(companyId));
        return Ok(result);
    }

    /// <summary>
    /// Reveal and decrypt a specific portal credential and secret notes
    /// </summary>
    [HttpGet("credentials/{id:guid}/reveal")]
    [ProducesResponseType(typeof(DecryptedCredentialDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<DecryptedCredentialDto>> RevealCredential(Guid id)
    {
        var result = await _mediator.Send(new RevealCredentialQuery(id));
        return Ok(result);
    }

    /// <summary>
    /// Encrypt and store a new portal credential
    /// </summary>
    [HttpPost("credentials")]
    [ProducesResponseType(typeof(CredentialDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<CredentialDto>> CreateCredential([FromBody] CreateCredentialCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetCredentials), new { id = result.Id }, result);
    }

    /// <summary>
    /// Update an existing portal credential
    /// </summary>
    [HttpPut("credentials/{id:guid}")]
    [ProducesResponseType(typeof(CredentialDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CredentialDto>> UpdateCredential(Guid id, [FromBody] UpdateCredentialCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("Path id and command id do not match.");
        }
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Delete a portal credential
    /// </summary>
    [HttpDelete("credentials/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteCredential(Guid id)
    {
        await _mediator.Send(new DeleteCredentialCommand(id));
        return NoContent();
    }
}
