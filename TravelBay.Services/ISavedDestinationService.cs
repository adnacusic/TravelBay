using TravelBay.Model.Requests;
using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;

namespace TravelBay.Services;

/// <summary>Current-user scoped.</summary>
public interface ISavedDestinationService
{
    /// <summary>Newest first, paged like every list.</summary>
    Task<PageResult<SavedDestinationResponse>> GetAllAsync(SavedDestinationSearchObject? search);
    Task<SavedDestinationResponse> AddAsync(SavedDestinationInsertRequest request);
    Task RemoveAsync(int id);
}
