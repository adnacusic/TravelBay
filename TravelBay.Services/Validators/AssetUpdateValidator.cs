using TravelBay.Model.Requests;
using FluentValidation;

namespace TravelBay.Services.Validators
{
    public class AssetUpdateValidator : AbstractValidator<AssetUpdateRequest>
    {
        public AssetUpdateValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id is required and must be greater than 0.");

            RuleFor(x => x.FileName)
                .NotEmpty().WithMessage("FileName is required.")
                .MaximumLength(100).WithMessage("FileName cannot exceed 100 characters.");

            RuleFor(x => x.ContentType)
                .NotEmpty().WithMessage("ContentType is required.")
                .MaximumLength(100).WithMessage("ContentType cannot exceed 100 characters.");

            RuleFor(x => x.Base64Content)
                .NotEmpty().WithMessage("Base64Content is required.");
        }
    }
}
