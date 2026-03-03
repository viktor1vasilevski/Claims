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
    public async Task<IEnumerable<ClaimDto>> GetAsync()
        => await _claimsService.GetClaimsAsync();

    [HttpPost]
    public async Task<ActionResult> CreateAsync(CreateClaimRequest request)
    {
        var result = await _claimsService.CreateClaimAsync(request);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task DeleteAsync(string id)
        => await _claimsService.DeleteClaimAsync(id);

    [HttpGet("{id}")]
    public async Task<ClaimDto?> GetAsync(string id)
        => await _claimsService.GetClaimAsync(id);
}