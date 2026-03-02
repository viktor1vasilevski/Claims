using Claims.Application.DTOs;
using Claims.Domain.Models;

namespace Claims.Application.Interfaces;

public interface IClaimsService
{
    Task<List<ClaimDto>> GetClaimsAsync();
    Task<ClaimDto> GetClaimAsync(string id);
    Task CreateClaimAsync(Claim claim);
    Task DeleteClaimAsync(string id);
}
