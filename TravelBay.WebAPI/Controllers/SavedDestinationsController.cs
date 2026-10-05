using TravelBay.Model.Requests;
using TravelBay.Model.Responses;
using TravelBay.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TravelBay.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class SavedDestinationsController : ControllerBase
{
    private readonly ISavedDestinationService _savedDestinationService;

    public SavedDestinationsController(ISavedDestinationService savedDestinationService)
    {
        _savedDestinationService = savedDestinationService;
    }

    [HttpGet]
    public async Task<ActionResult<List<SavedDestinationResponse>>> GetAll() => Ok(await _savedDestinationService.GetAllAsync());

    [HttpPost]
    public async Task<ActionResult<SavedDestinationResponse>> Add([FromBody] SavedDestinationInsertRequest request) =>
        Ok(await _savedDestinationService.AddAsync(request));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Remove(int id)
    {
        await _savedDestinationService.RemoveAsync(id);
        return NoContent();
    }
}
