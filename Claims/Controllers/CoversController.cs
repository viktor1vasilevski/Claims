using Claims.Application.DTOs;
using Claims.Application.Interfaces;
using Claims.Application.Mappers;
using Claims.Application.Requests.Cover;
using Microsoft.AspNetCore.Mvc;

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
        var covers = await coversService.GetCoversAsync(cancellationToken);
        return Ok(covers.Select(CoverMapper.ToDto));
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
    public async Task<ActionResult<CoverDto>> GetById(string id, CancellationToken cancellationToken)
    {
        var cover = await coversService.GetCoverByIdAsync(id, cancellationToken);
        return cover is null ? NotFound() : Ok(CoverMapper.ToDto(cover));
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
        var cover = await coversService.CreateCoverAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = cover.Id }, CoverMapper.ToDto(cover));
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
    public async Task<ActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        await coversService.DeleteCoverAsync(id, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Computes the premium for a cover without creating it.
    /// </summary>
    /// <param name="request">The cover details used to compute the premium.</param>
    /// <returns>The computed premium amount.</returns>
    [HttpGet("compute")]
    [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult ComputePremium([FromQuery] ComputePremiumRequest request)
    {
        var result = coversService.ComputePremium(request);
        return Ok(result);
    }
}