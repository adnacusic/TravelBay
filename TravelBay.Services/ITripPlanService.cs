using TravelBay.Model.Requests;
using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;

namespace TravelBay.Services;

public interface ITripPlanService : IBaseCRUDService<TripPlanResponse, TripPlanSearchObject, TripPlanInsertRequest, TripPlanUpdateRequest>
{
    Task<List<TripPlanItemResponse>> GetItemsAsync(int tripPlanId);
    Task<TripPlanItemResponse> AddItemAsync(int tripPlanId, TripPlanItemInsertRequest request);
    Task<TripPlanItemResponse> UpdateItemAsync(int tripPlanId, int itemId, TripPlanItemUpdateRequest request);
    Task RemoveItemAsync(int tripPlanId, int itemId);
}
