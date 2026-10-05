using TravelBay.Model.Requests;
using FluentValidation;

namespace TravelBay.Services.Validators
{
    public class NewsUpdateValidator : AbstractValidator<NewsUpdateRequest>
    {
        public NewsUpdateValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Content).NotEmpty();
            RuleFor(x => x.ImageUrl).NotEmpty().MaximumLength(500);
        }
    }
}
