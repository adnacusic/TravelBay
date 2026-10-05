using TravelBay.Model.Requests;
using FluentValidation;

namespace TravelBay.Services.Validators
{
    public class CollectionUpdateValidator : AbstractValidator<CollectionUpdateRequest>
    {
        public CollectionUpdateValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        }
    }
}
