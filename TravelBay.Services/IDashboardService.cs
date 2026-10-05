using TravelBay.Model.Responses;

namespace TravelBay.Services;

public interface IDashboardService
{
    Task<DashboardStatsResponse> GetStatsAsync();
}
