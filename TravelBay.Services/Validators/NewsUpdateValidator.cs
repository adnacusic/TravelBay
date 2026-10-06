using TravelBay.Model.Requests;

namespace TravelBay.Services.Validators
{
    public class NewsUpdateValidator : NewsRequestValidator<NewsUpdateRequest>
    {
        public NewsUpdateValidator() : base(imageRequired: false)
        {
        }
    }
}
