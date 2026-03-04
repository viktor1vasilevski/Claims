using Claims.Application.DTOs;
using Claims.Domain.Models;

namespace Claims.Application.Mappers;

public static class ClaimMapper
{
    public static ClaimDto ToDto(Claim claim) => new()
    {
        Id = claim.Id,
        CoverId = claim.CoverId,
        Created = claim.Created,
        Name = claim.Name,
        Type = claim.Type,
        DamageCost = claim.DamageCost
    };
}
