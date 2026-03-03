using Claims.Application.Requests.Claims;
using FluentValidation;

namespace Claims.Application.Validations.Claims;

public class CreateClaimRequestValidator : AbstractValidator<CreateClaimRequest>
{
    public CreateClaimRequestValidator()
    {
        RuleFor(x => x.DamageCost)
            .LessThanOrEqualTo(100_000)
            .WithMessage("DamageCost cannot exceed 100,000.");

        RuleFor(x => x.Created)
            .NotEmpty()
            .WithMessage("Created date is required.");
    }
}
