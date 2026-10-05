using TravelBay.Model.Requests;
using TravelBay.Model.Responses;

namespace TravelBay.Services;

/// <summary>Current-user scoped.</summary>
public interface ISavedDestinationService
{
    Task<List<SavedDestinationResponse>> GetAllAsync();
    Task<SavedDestinationResponse> AddAsync(SavedDestinationInsertRequest request);
    Task RemoveAsync(int id);
}
