using Claims.Application.Requests.Cover;
using FluentValidation;

namespace Claims.Application.Validations.Cover;

public class CreateCoverRequestValidator : AbstractValidator<CreateCoverRequest>
{
    public CreateCoverRequestValidator()
    {
        RuleFor(x => x.StartDate)
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
            .WithMessage("StartDate cannot be in the past.");

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate)
            .WithMessage("EndDate must be after StartDate.")
            .Must((request, endDate) => (endDate - request.StartDate).TotalDays <= 365)
            .WithMessage("Insurance period cannot exceed 1 year.");
    }
}
