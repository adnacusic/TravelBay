using TravelBay.Model.Requests;
using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;

namespace TravelBay.Services
{
    public interface IDestinationService : IBaseCRUDService<DestinationResponse, DestinationSearchObject, DestinationInsertRequest, DestinationUpdateRequest>
    {
        /// <summary>Top N by views, then approved reviews, then average rating (N is 1-50, default 10).</summary>
        Task<PageResult<DestinationResponse>> GetPopularAsync(int? top);
    }
}
