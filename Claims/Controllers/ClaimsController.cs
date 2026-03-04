using Claims.Application.DTOs;
using Claims.Application.Interfaces;
using Claims.Application.Requests.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers;

[ApiController]
[Route("[controller]")]
public class ClaimsController(IClaimsService _claimsService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClaimDto>>> GetAsync()
    {
        var result = await _claimsService.GetClaimsAsync();
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ClaimDto>> CreateAsync(CreateClaimRequest request)
    {
        var result = await _claimsService.CreateClaimAsync(request);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAsync(string id)
    {
        await _claimsService.DeleteClaimAsync(id);
        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ClaimDto>> GetAsync(string id)
    {
        var result = await _claimsService.GetClaimAsync(id);
        return result is null ? NotFound() : Ok(result);
    }
}