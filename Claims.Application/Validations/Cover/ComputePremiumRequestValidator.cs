using Claims.Application.Requests.Cover;
using FluentValidation;

namespace Claims.Application.Validations.Cover;

public class ComputePremiumRequestValidator : AbstractValidator<ComputePremiumRequest>
{
    public ComputePremiumRequestValidator()
    {
        RuleFor(x => x.StartDate)
            .GreaterThanOrEqualTo(_ => DateTime.UtcNow.Date)
            .WithMessage("StartDate cannot be in the past.");

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate)
            .WithMessage("EndDate must be after StartDate.")
            .Must((request, endDate) => (endDate - request.StartDate).TotalDays <= 365)
            .WithMessage("Insurance period cannot exceed 1 year.");

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Cover type is invalid.");
    }
}
