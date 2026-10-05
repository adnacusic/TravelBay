using TravelBay.Model.Requests;
using FluentValidation;

namespace TravelBay.Services.Validators;

public class ReviewUpdateValidator : AbstractValidator<ReviewUpdateRequest>
{
    public ReviewUpdateValidator()
    {
        RuleFor(x => x.Rating).InclusiveBetween(1, 5);
        RuleFor(x => x.Comment).MaximumLength(1000);
    }
}
