using Claims.Domain.Enums;

namespace Claims.Application.Requests.Claims;

public class CreateClaimRequest
{
    public string CoverId { get; set; }
    public DateTime Created { get; set; }
    public string Name { get; set; }
    public ClaimType Type { get; set; }
    public decimal DamageCost { get; set; }
}
