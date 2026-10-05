using TravelBay.Model.Requests;
using TravelBay.Model.Responses;

namespace TravelBay.Services;

/// <summary>Current-user scoped.</summary>
public interface IUserPreferenceService
{
    Task<List<UserPreferenceResponse>> GetAllAsync();
    Task<List<UserPreferenceResponse>> SetAsync(SetUserPreferencesRequest request);
}
