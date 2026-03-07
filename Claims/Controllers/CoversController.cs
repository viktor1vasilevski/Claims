using Claims.Api.Mappers;
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
    /// Retrieves all covers.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CoverDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CoverDto>>> GetAsync(CancellationToken cancellationToken)
    {
        var covers = await _coversService.GetCoversAsync(cancellationToken);
        return Ok(covers.Select(CoverMapper.ToDto));
    }

    /// <summary>
    /// Retrieves a cover by ID.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CoverDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CoverDto>> GetAsync(string id, CancellationToken cancellationToken)
    {
        var cover = await _coversService.GetCoverAsync(id, cancellationToken);
        return cover is null ? NotFound() : Ok(CoverMapper.ToDto(cover));
    }

    /// <summary>
    /// Creates a new cover.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CoverDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CoverDto>> CreateAsync(CreateCoverRequest request, CancellationToken cancellationToken)
    {
        var cover = await _coversService.CreateCoverAsync(request, cancellationToken);
        return Ok(CoverMapper.ToDto(cover));
    }

    /// <summary>
    /// Deletes a cover by ID.
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        await _coversService.DeleteCoverAsync(id, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Computes the premium for a cover.
    /// </summary>
    [HttpPost("compute")]
    [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
    public async Task<ActionResult> ComputePremiumAsync(DateTime startDate, DateTime endDate, CoverType coverType)
    {
        var result = await _coversService.ComputePremiumAsync(startDate, endDate, coverType);
        return Ok(result);
    }
}