using TravelBay.Model.Requests;
using FluentValidation;

namespace TravelBay.Services.Validators
{
    public class CollectionInsertValidator : AbstractValidator<CollectionInsertRequest>
    {
        public CollectionInsertValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        }
    }
}
