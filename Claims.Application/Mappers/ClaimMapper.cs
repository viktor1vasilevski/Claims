using Claims.Application.DTOs;
using Claims.Domain.Models;

namespace Claims.Application.Mappers;

/// <summary>
/// Maps <see cref="Claim"/> domain models to <see cref="ClaimDto"/> data transfer objects.
/// </summary>
public static class ClaimMapper
{
    /// <summary>
    /// Maps a <see cref="Claim"/> to a <see cref="ClaimDto"/>.
    /// </summary>
    /// <param name="claim">The claim domain model to map.</param>
    /// <returns>A <see cref="ClaimDto"/> representing the claim.</returns>
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
