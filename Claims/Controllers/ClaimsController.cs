using Claims.Application.Requests.Claims;

namespace Claims.Controllers;

/// <summary>
/// Manages insurance claims.
/// </summary>
[ApiController]
[Route("[controller]")]
public class ClaimsController(IClaimsService claimsService) : ControllerBase
{
    /// <summary>
    /// Retrieves all claims.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the request.</param>
    /// <returns>A list of claims.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ClaimDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ClaimDto>>> Get(CancellationToken cancellationToken)
    {
        var response = await claimsService.GetClaimsAsync(cancellationToken);
        return Ok(response);
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
    public async Task<ActionResult<ClaimDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var response = await claimsService.GetClaimByIdAsync(id, cancellationToken);
        return response is null ? NotFound() : Ok(response);
    }

    /// <summary>
    /// Creates a new claim.
    /// </summary>
    /// <param name="request">The claim details.</param>
    /// <param name="cancellationToken">Token to cancel the request.</param>
    /// <returns>The created claim.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ClaimDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClaimDto>> Create(CreateClaimRequest request, CancellationToken cancellationToken)
    {
        var response = await claimsService.CreateClaimAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    /// <summary>
    /// Deletes a claim by ID.
    /// </summary>
    /// <param name="id">The claim ID.</param>
    /// <param name="cancellationToken">Token to cancel the request.</param>
    /// <returns>No content if deleted, 404 if not found.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await claimsService.DeleteClaimAsync(id, cancellationToken);
        return NoContent();
    }
}