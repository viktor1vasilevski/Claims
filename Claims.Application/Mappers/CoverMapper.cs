using Claims.Application.DTOs;
using Claims.Domain.Models;

namespace Claims.Application.Mappers;

/// <summary>
/// Maps <see cref="Cover"/> domain models to <see cref="CoverDto"/> data transfer objects.
/// </summary>
public static class CoverMapper
{
    /// <summary>
    /// Maps a <see cref="Cover"/> to a <see cref="CoverDto"/>.
    /// </summary>
    /// <param name="cover">The cover domain model to map.</param>
    /// <returns>A <see cref="CoverDto"/> representing the cover.</returns>
    public static CoverDto ToDto(Cover cover) => new()
    {
        Id = cover.Id,
        StartDate = cover.StartDate,
        EndDate = cover.EndDate,
        Type = cover.Type,
        Premium = cover.Premium
    };
}
