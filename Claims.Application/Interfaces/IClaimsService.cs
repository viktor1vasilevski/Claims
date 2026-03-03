using Claims.Application.DTOs;
using Claims.Application.Requests.Claims;

namespace Claims.Application.Interfaces;

public interface IClaimsService
{
    Task<List<ClaimDto>> GetClaimsAsync();
    Task<ClaimDto> GetClaimAsync(string id);
    Task<ClaimDto> CreateClaimAsync(CreateClaimRequest request);
    Task DeleteClaimAsync(string id);
}
