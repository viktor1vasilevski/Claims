using Claims.Domain.Exceptions;

namespace Claims.Domain.Models;

public class Claim
{
    public Guid Id { get; private set; }
    public Guid CoverId { get; private set; }
    public DateTime Created { get; private set; }
    public string Name { get; private set; } = default!;
    public ClaimType Type { get; private set; }
    public decimal DamageCost { get; private set; }

    private Claim() { }

    public static Claim Create(Guid coverId, string name, ClaimType type, decimal damageCost, DateTime created)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidClaimNameException();

        if (damageCost <= 0 || damageCost > 100_000)
            throw new InvalidDamageCostException(damageCost);

        return new Claim
        {
            Id = Guid.NewGuid(),
            CoverId = coverId,
            Name = name,
            Type = type,
            DamageCost = damageCost,
            Created = created
        };
    }
}
