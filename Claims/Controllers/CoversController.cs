using Claims.Application.DTOs;
using Claims.Application.Interfaces;
using Claims.Application.Mappers;
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
    public async Task<ActionResult<decimal>> ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType)
    {
        var result = await _coversService.ComputePremiumAsync(startDate, endDate, coverType);
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
        var covers = await _coversService.GetCoversAsync();
        return Ok(covers.Select(CoverMapper.ToDto));
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
        var cover = await _coversService.GetCoverAsync(id);
        return cover is null ? NotFound() : Ok(CoverMapper.ToDto(cover));
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
        var cover = await _coversService.CreateCoverAsync(request);
        return Ok(CoverMapper.ToDto(cover));
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