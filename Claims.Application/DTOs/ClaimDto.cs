using Claims.Domain.Enums;

namespace Claims.Application.DTOs;

public class ClaimDto
{
    public string? Id { get; set; }
    public required string CoverId { get; set; }
    public DateTime Created { get; set; }
    public required string Name { get; set; }
    public ClaimType Type { get; set; }
    public decimal DamageCost { get; set; }
}
