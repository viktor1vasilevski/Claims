using Claims.Application.DTOs;
using Claims.Application.Interfaces;
using Claims.Domain.Models;
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
    public async Task<ActionResult> CreateAsync(Claim claim)
    {
        await _claimsService.CreateClaimAsync(claim);
        return Ok(claim);
    }

    [HttpDelete("{id}")]
    public async Task DeleteAsync(string id)
        => await _claimsService.DeleteClaimAsync(id);

    [HttpGet("{id}")]
    public async Task<ClaimDto?> GetAsync(string id)
        => await _claimsService.GetClaimAsync(id);
}