using TravelBay.Model.Requests;
using FluentValidation;

namespace TravelBay.Services.Validators
{
    public class DestinationInsertValidator : AbstractValidator<DestinationInsertRequest>
    {
        public DestinationInsertValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(200).WithMessage("Name cannot exceed 200 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters.");

            RuleFor(x => x.CategoryId).GreaterThan(0).WithMessage("CategoryId is required.");
            RuleFor(x => x.CityId).GreaterThan(0).WithMessage("CityId is required.");
        }
    }
}
