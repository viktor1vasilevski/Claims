
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
    => Ok(_coversService.ComputePremium(startDate, endDate, coverType));

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CoverDto>>> GetAsync()
        => Ok(await _coversService.GetCoversAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<CoverDto>> GetAsync(string id)
        => Ok(await _coversService.GetCoverAsync(id));

    [HttpPost]
    public async Task<ActionResult> CreateAsync(CreateCoverRequest request)
    {
        var result = await _coversService.CreateCoverAsync(request);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task DeleteAsync(string id)
        => await _coversService.DeleteCoverAsync(id);

}
