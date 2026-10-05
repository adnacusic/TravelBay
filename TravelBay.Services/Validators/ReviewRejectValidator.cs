using TravelBay.Model.Requests;
using FluentValidation;

namespace TravelBay.Services.Validators
{
    public class ReviewRejectValidator : AbstractValidator<ReviewRejectRequest>
    {
        public ReviewRejectValidator()
        {
            RuleFor(x => x.Reason)
                .NotEmpty().WithMessage("A reason is required to reject a review.")
                .MaximumLength(500);
        }
    }
}
