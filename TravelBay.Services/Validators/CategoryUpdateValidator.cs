

using TravelBay.Model.Requests;
using FluentValidation;

namespace TravelBay.Services.Validators
{
    public class CategoryUpdateValidator : AbstractValidator<CategoriesUpdateRequest>
    {
        public CategoryUpdateValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(50).WithMessage("Name cannot exceed 50 characters.")
                .MinimumLength(4).WithMessage("Name must have at least 4 characters");
        }
    }
}
