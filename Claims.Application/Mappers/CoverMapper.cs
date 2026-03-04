using Claims.Application.DTOs;
using Claims.Domain.Models;

namespace Claims.Application.Mappers;

public static class CoverMapper
{
    public static CoverDto ToDto(Cover cover) => new()
    {
        Id = cover.Id,
        StartDate = cover.StartDate,
        EndDate = cover.EndDate,
        Type = cover.Type,
        Premium = cover.Premium
    };
}
