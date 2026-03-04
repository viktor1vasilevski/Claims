using Claims.Application.DTOs;
using Claims.Application.Interfaces;
using Claims.Application.Requests.Cover;
using Claims.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers;

[ApiController]
[Route("[controller]")]
public class CoversController(ICoversService _coversService) : ControllerBase
{
    [HttpPost("compute")]
    public ActionResult ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType)
    {
        var result = _coversService.ComputePremium(startDate, endDate, coverType);
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CoverDto>>> GetAsync()
    {
        var result = await _coversService.GetCoversAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CoverDto>> GetAsync(string id)
    {
        var result = await _coversService.GetCoverAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<CoverDto>> CreateAsync(CreateCoverRequest request)
    {
        var result = await _coversService.CreateCoverAsync(request);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAsync(string id)
    {
        await _coversService.DeleteCoverAsync(id);
        return NoContent();
    }
}