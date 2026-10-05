using TravelBay.Model.Requests;
using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;
using TravelBay.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TravelBay.WebAPI.Controllers;

[Authorize]
public class CollectionsController : BaseCRUDController<CollectionResponse, CollectionSearchObject, CollectionInsertRequest, CollectionUpdateRequest, ICollectionService>
{
    public CollectionsController(ICollectionService collectionService) : base(collectionService)
    {
    }

    [HttpGet("{id}/Items")]
    public async Task<ActionResult<List<CollectionItemResponse>>> GetItems(int id) => Ok(await _service.GetItemsAsync(id));

    [HttpPost("{id}/Items")]
    public async Task<ActionResult<CollectionItemResponse>> AddItem(int id, [FromBody] CollectionItemInsertRequest request) =>
        Ok(await _service.AddItemAsync(id, request));

    [HttpDelete("{id}/Items/{itemId}")]
    public async Task<IActionResult> RemoveItem(int id, int itemId)
    {
        await _service.RemoveItemAsync(id, itemId);
        return NoContent();
    }
}
