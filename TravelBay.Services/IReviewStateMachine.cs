using TravelBay.Model.Responses;

namespace TravelBay.Services;

public interface IReviewStateMachine
{
    Task<ReviewResponse> ApproveAsync(int reviewId);
    Task<ReviewResponse> RejectAsync(int reviewId, string reason);
}
