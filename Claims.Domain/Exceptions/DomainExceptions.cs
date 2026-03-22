namespace Claims.Domain.Exceptions;

public class CoverNotFoundException(Guid coverId)
    : Exception($"Cover with id '{coverId}' was not found.");

public class ClaimDateOutOfRangeException()
    : Exception("Created date must be within the Cover period.");

public class CoverHasActiveClaimsException(Guid coverId)
    : Exception($"Cannot delete cover '{coverId}' because it has active claims.");

public class PremiumStrategyNotFoundException(CoverType coverType)
    : Exception($"No premium strategy found for cover type {coverType}.");

public class ClaimNotFoundException(Guid id)
    : Exception($"Claim with id '{id}' was not found.");

public class UnhandledAuditEntityTypeException(AuditEntityType entityType)
    : Exception($"Unhandled audit entity type: {entityType}");

public class InvalidDamageCostException(decimal damageCost)
    : Exception($"DamageCost '{damageCost}' is invalid. Must be greater than 0 and no more than 100,000.");

public class InvalidCoverPeriodException(string reason)
    : Exception(reason);
