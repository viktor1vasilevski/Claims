namespace Claims.Application.Validations.Claims;

public class CreateClaimRequestValidator : AbstractValidator<CreateClaimRequest>
{
    public CreateClaimRequestValidator()
    {
        RuleFor(x => x.CoverId)
            .NotEmpty()
            .WithMessage("CoverId is required.");

        RuleFor(x => x.Name)
            .Must(name => !string.IsNullOrWhiteSpace(name))
            .WithMessage("Name is required.");

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Claim type is invalid.");

        RuleFor(x => x.DamageCost)
            .GreaterThan(0)
            .WithMessage("DamageCost must be greater than 0.")
            .LessThanOrEqualTo(100_000)
            .WithMessage("DamageCost cannot exceed 100,000.");

        RuleFor(x => x.Created)
            .NotEmpty()
            .WithMessage("Created date is required.");
    }
}
