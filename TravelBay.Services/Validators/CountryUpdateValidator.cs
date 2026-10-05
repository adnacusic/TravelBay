using TravelBay.Model.Requests;
using FluentValidation;

namespace TravelBay.Services.Validators
{
    public class CountryUpdateValidator : AbstractValidator<CountryUpdateRequest>
    {
        public CountryUpdateValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        }
    }
}
