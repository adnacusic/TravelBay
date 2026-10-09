using TravelBay.Model.Responses;
using TravelBay.Services.Database;

namespace TravelBay.Services;

public interface IReviewStateMachine
{
    Task<ReviewResponse> ApproveAsync(int reviewId);
    Task<ReviewResponse> RejectAsync(int reviewId, string reason);

    /// <summary>
    /// The author edits their review: a Pending one stays Pending, an Approved one goes back to Pending
    /// (the new text must be moderated again before it is public), a Rejected one cannot be edited —
    /// the user writes a new review instead. Adds the audit entry; the caller saves.
    /// </summary>
    void PrepareForEdit(Review review, int authorId);
}
