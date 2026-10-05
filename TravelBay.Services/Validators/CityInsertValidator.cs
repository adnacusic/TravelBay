using TravelBay.Model.Requests;
using FluentValidation;

namespace TravelBay.Services.Validators
{
    public class CityInsertValidator : AbstractValidator<CityInsertRequest>
    {
        public CityInsertValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.CountryId).GreaterThan(0);
        }
    }
}
