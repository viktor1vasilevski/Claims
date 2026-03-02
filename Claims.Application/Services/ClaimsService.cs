using Claims.Application.DTOs;
using Claims.Application.Interfaces;
using Claims.Domain.Models;

namespace Claims.Application.Services;

public class ClaimsService(IClaimsRepository _claimsRepository) : IClaimsService
{
    public async Task CreateClaimAsync(Claim claim)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteClaimAsync(string id)
    {
        throw new NotImplementedException();
    }

    public async Task<ClaimDto> GetClaimAsync(string id)
    {
        throw new NotImplementedException();
    }

    public async Task<List<ClaimDto>> GetClaimsAsync()
    {
        throw new NotImplementedException();
    }
}
