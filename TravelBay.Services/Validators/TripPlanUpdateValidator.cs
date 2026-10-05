using TravelBay.Model.Requests;
using FluentValidation;

namespace TravelBay.Services.Validators
{
    public class TripPlanUpdateValidator : AbstractValidator<TripPlanUpdateRequest>
    {
        public TripPlanUpdateValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.EndDate).GreaterThanOrEqualTo(x => x.StartDate)
                .WithMessage("EndDate must be on or after StartDate.");
        }
    }
}
