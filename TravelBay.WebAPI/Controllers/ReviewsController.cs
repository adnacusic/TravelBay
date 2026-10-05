using TravelBay.Model.Constants;
using TravelBay.Model.Requests;
using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;
using TravelBay.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TravelBay.WebAPI.Controllers;

[Authorize]
public class ReviewsController
    : BaseCRUDController<ReviewResponse, ReviewSearchObject, ReviewInsertRequest, ReviewUpdateRequest, IReviewService>
{
    private readonly IReviewStateMachine _stateMachine;

    public ReviewsController(IReviewService reviewService, IReviewStateMachine stateMachine)
        : base(reviewService)
    {
        _stateMachine = stateMachine;
    }

    [Authorize(Roles = RoleNames.Admin)]
    [HttpPost("{id}/Approve")]
    public async Task<ActionResult<ReviewResponse>> Approve(int id) => Ok(await _stateMachine.ApproveAsync(id));

    [Authorize(Roles = RoleNames.Admin)]
    [HttpPost("{id}/Reject")]
    public async Task<ActionResult<ReviewResponse>> Reject(int id, [FromBody] ReviewRejectRequest request) =>
        Ok(await _stateMachine.RejectAsync(id, request.Reason));
}
