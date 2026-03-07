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
    /// <param name="cancellationToken">Token to cancel the request.</param>
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
    /// <param name="id">The cover ID.</param>
    /// <param name="cancellationToken">Token to cancel the request.</param>
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
    /// <param name="request">The cover details.</param>
    /// <param name="cancellationToken">Token to cancel the request.</param>
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
    /// <param name="id">The cover ID.</param>
    /// <param name="cancellationToken">Token to cancel the request.</param>
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
    /// <param name="startDate">The cover start date.</param>
    /// <param name="endDate">The cover end date.</param>
    /// <param name="coverType">The type of cover.</param>
    [HttpPost("compute")]
    [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
    public async Task<ActionResult> ComputePremiumAsync(DateTime startDate, DateTime endDate, CoverType coverType)
    {
        var result = await _coversService.ComputePremiumAsync(startDate, endDate, coverType);
        return Ok(result);
    }
}