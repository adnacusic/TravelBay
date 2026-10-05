using TravelBay.Model.Requests;
using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;
using TravelBay.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TravelBay.WebAPI.Controllers;

[Authorize]
public class TripPlansController : BaseCRUDController<TripPlanResponse, TripPlanSearchObject, TripPlanInsertRequest, TripPlanUpdateRequest, ITripPlanService>
{
    private readonly ITripPlanStateMachine _stateMachine;

    public TripPlansController(ITripPlanService tripPlanService, ITripPlanStateMachine stateMachine) : base(tripPlanService)
    {
        _stateMachine = stateMachine;
    }

    [HttpPost("{id}/Activate")]
    public async Task<ActionResult<TripPlanResponse>> Activate(int id) => Ok(await _stateMachine.ActivateAsync(id));

    [HttpPost("{id}/Complete")]
    public async Task<ActionResult<TripPlanResponse>> Complete(int id) => Ok(await _stateMachine.CompleteAsync(id));

    [HttpPost("{id}/Cancel")]
    public async Task<ActionResult<TripPlanResponse>> Cancel(int id) => Ok(await _stateMachine.CancelAsync(id));

    [HttpGet("{id}/Items")]
    public async Task<ActionResult<List<TripPlanItemResponse>>> GetItems(int id) => Ok(await _service.GetItemsAsync(id));

    [HttpPost("{id}/Items")]
    public async Task<ActionResult<TripPlanItemResponse>> AddItem(int id, [FromBody] TripPlanItemInsertRequest request) =>
        Ok(await _service.AddItemAsync(id, request));

    [HttpPut("{id}/Items/{itemId}")]
    public async Task<ActionResult<TripPlanItemResponse>> UpdateItem(int id, int itemId, [FromBody] TripPlanItemUpdateRequest request) =>
        Ok(await _service.UpdateItemAsync(id, itemId, request));

    [HttpDelete("{id}/Items/{itemId}")]
    public async Task<IActionResult> RemoveItem(int id, int itemId)
    {
        await _service.RemoveItemAsync(id, itemId);
        return NoContent();
    }
}
