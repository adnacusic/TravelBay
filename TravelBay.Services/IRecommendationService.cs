using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;

namespace TravelBay.Services;

/// <summary>Current-user scoped (user id comes from the JWT).</summary>
public interface IRecommendationService
{
    Task<PageResult<RecommendationResponse>> GetRecommendationsAsync(RecommendationSearchObject? search = null);
}
