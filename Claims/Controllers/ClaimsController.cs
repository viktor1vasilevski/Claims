using Claims.Application.DTOs;
using Claims.Application.Interfaces;
using Claims.Application.Requests.Claims;
using Microsoft.AspNetCore.Mvc;

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
    /// <returns>A list of claims.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ClaimDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ClaimDto>>> GetAsync()
    {
        var result = await _claimsService.GetClaimsAsync();
        return Ok(result);
    }

    /// <summary>
    /// Creates a new claim.
    /// </summary>
    /// <param name="request">The claim details.</param>
    /// <returns>The created claim.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ClaimDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ClaimDto>> CreateAsync(CreateClaimRequest request)
    {
        var result = await _claimsService.CreateClaimAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Deletes a claim by ID.
    /// </summary>
    /// <param name="id">The claim ID.</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteAsync(string id)
    {
        await _claimsService.DeleteClaimAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Retrieves a claim by ID.
    /// </summary>
    /// <param name="id">The claim ID.</param>
    /// <returns>The claim if found.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ClaimDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClaimDto>> GetAsync(string id)
    {
        var result = await _claimsService.GetClaimAsync(id);
        return result is null ? NotFound() : Ok(result);
    }
}