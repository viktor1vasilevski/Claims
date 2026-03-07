using Claims.Domain.Enums;

namespace Claims.Domain.Exceptions;

public class CoverNotFoundException(string coverId)
    : Exception($"Cover with id '{coverId}' was not found.");

public class ClaimDateOutOfRangeException()
    : Exception("Created date must be within the Cover period.");

public class CoverHasActiveClaimsException(string coverId)
    : Exception($"Cannot delete cover '{coverId}' because it has active claims.");

public class PremiumStrategyNotFoundException(CoverType coverType)
    : Exception($"No premium strategy found for cover type {coverType}.");

public class UnhandledAuditEntityTypeException(AuditEntityType entityType)
    : Exception($"Unhandled audit entity type: {entityType}");

public class ClaimNotFoundException(string id)
    : Exception($"Claim with id '{id}' was not found.");
