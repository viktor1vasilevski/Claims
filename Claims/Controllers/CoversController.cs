using Claims.Application.DTOs;
using Claims.Application.Interfaces;
using Claims.Application.Requests.Cover;
using Claims.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers;

/// <summary>
/// Manages insurance covers.
/// </summary>
[ApiController]
[Route("[controller]")]
public class CoversController(ICoversService _coversService) : ControllerBase
{
    /// <summary>
    /// Computes the premium for a cover.
    /// </summary>
    /// <param name="startDate">The start date of the cover.</param>
    /// <param name="endDate">The end date of the cover.</param>
    /// <param name="coverType">The type of cover.</param>
    /// <returns>The computed premium.</returns>
    [HttpPost("compute")]
    [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
    public ActionResult ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType)
    {
        var result = _coversService.ComputePremium(startDate, endDate, coverType);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves all covers.
    /// </summary>
    /// <returns>A list of covers.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CoverDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CoverDto>>> GetAsync()
    {
        var result = await _coversService.GetCoversAsync();
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a cover by ID.
    /// </summary>
    /// <param name="id">The cover ID.</param>
    /// <returns>The cover if found.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CoverDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CoverDto>> GetAsync(string id)
    {
        var result = await _coversService.GetCoverAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Creates a new cover.
    /// </summary>
    /// <param name="request">The cover details.</param>
    /// <returns>The created cover.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(CoverDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CoverDto>> CreateAsync(CreateCoverRequest request)
    {
        var result = await _coversService.CreateCoverAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Deletes a cover by ID.
    /// </summary>
    /// <param name="id">The cover ID.</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteAsync(string id)
    {
        await _coversService.DeleteCoverAsync(id);
        return NoContent();
    }
}