using Claims.Application.Requests.Cover;

namespace Claims.Controllers;

/// <summary>
/// Manages insurance covers.
/// </summary>
[ApiController]
[Route("[controller]")]
public class CoversController(ICoversService coversService) : ControllerBase
{
    /// <summary>
    /// Retrieves all covers.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the request.</param>
    /// <returns>A list of all covers.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CoverDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CoverDto>>> Get(CancellationToken cancellationToken)
    {
        var response = await coversService.GetCoversAsync(cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Retrieves a cover by ID.
    /// </summary>
    /// <param name="id">The cover ID.</param>
    /// <param name="cancellationToken">Token to cancel the request.</param>
    /// <returns>The cover if found, otherwise 404 Not Found.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CoverDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CoverDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var response = await coversService.GetCoverByIdAsync(id, cancellationToken);
        return response is null ? NotFound() : Ok(response);
    }

    /// <summary>
    /// Creates a new cover.
    /// </summary>
    /// <param name="request">The cover details.</param>
    /// <param name="cancellationToken">Token to cancel the request.</param>
    /// <returns>The created cover.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(CoverDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CoverDto>> Create(CreateCoverRequest request, CancellationToken cancellationToken)
    {
        var response = await coversService.CreateCoverAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    /// <summary>
    /// Deletes a cover by ID.
    /// </summary>
    /// <param name="id">The cover ID.</param>
    /// <param name="cancellationToken">Token to cancel the request.</param>
    /// <returns>No content if deleted, 404 if not found, 409 if cover has active claims.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await coversService.DeleteCoverAsync(id, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Computes the premium for a cover without creating it.
    /// </summary>
    /// <param name="request">The cover details used to compute the premium.</param>
    /// <returns>The computed premium amount.</returns>
    [HttpGet("premium")]
    [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult ComputePremium([FromQuery] ComputePremiumRequest request)
    {
        var response = coversService.ComputePremium(request);
        return Ok(response);
    }
}