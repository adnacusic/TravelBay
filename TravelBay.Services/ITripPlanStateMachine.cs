using TravelBay.Model.Responses;

namespace TravelBay.Services;

public interface ITripPlanStateMachine
{
    Task<TripPlanResponse> ActivateAsync(int tripPlanId);
    Task<TripPlanResponse> CompleteAsync(int tripPlanId);
    Task<TripPlanResponse> CancelAsync(int tripPlanId);
}
