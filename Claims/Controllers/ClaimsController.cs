using Claims.Application.Interfaces;
using Claims.Application.Requests.Claims;
using Claims.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using Claims.Api.Mappers;

namespace Claims.Controllers;

/// <summary>
/// Manages insurance claims.
/// </summary>
[ApiController]
[Route("[controller]")]
public class ClaimsController(IClaimsService _claimsService) : ControllerBase
{
    /// <summary>
    /// Retrieves all claims.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the request.</param>
    /// <returns>A list of claims.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ClaimDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ClaimDto>>> GetAsync(CancellationToken cancellationToken)
    {
        var claims = await _claimsService.GetClaimsAsync(cancellationToken);
        return Ok(claims.Select(ClaimMapper.ToDto));
    }

    /// <summary>
    /// Retrieves a claim by ID.
    /// </summary>
    /// <param name="id">The claim ID.</param>
    /// <param name="cancellationToken">Token to cancel the request.</param>
    /// <returns>The claim if found.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ClaimDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClaimDto>> GetAsync(string id, CancellationToken cancellationToken)
    {
        var claim = await _claimsService.GetClaimAsync(id, cancellationToken);
        return claim is null ? NotFound() : Ok(ClaimMapper.ToDto(claim));
    }

    /// <summary>
    /// Creates a new claim.
    /// </summary>
    /// <param name="request">The claim details.</param>
    /// <param name="cancellationToken">Token to cancel the request.</param>
    /// <returns>The created claim.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ClaimDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ClaimDto>> CreateAsync(CreateClaimRequest request, CancellationToken cancellationToken)
    {
        var claim = await _claimsService.CreateClaimAsync(request, cancellationToken);
        return Ok(ClaimMapper.ToDto(claim));
    }

    /// <summary>
    /// Deletes a claim by ID.
    /// </summary>
    /// <param name="id">The claim ID.</param>
    /// <param name="cancellationToken">Token to cancel the request.</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        await _claimsService.DeleteClaimAsync(id, cancellationToken);
        return NoContent();
    }
}