using TravelBay.Model.Constants;
using TravelBay.Model.Requests;
using TravelBay.Model.Responses;
using TravelBay.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TravelBay.WebAPI.Controllers;

[Authorize(Roles = RoleNames.Admin)]
[ApiController]
[Route("[controller]")]
public class DestinationImagesController : ControllerBase
{
    private readonly IDestinationImageService _destinationImageService;

    public DestinationImagesController(IDestinationImageService destinationImageService)
    {
        _destinationImageService = destinationImageService;
    }

    [HttpPost]
    public async Task<ActionResult<DestinationImageResponse>> Create([FromBody] DestinationImageInsertRequest request) =>
        Ok(await _destinationImageService.InsertAsync(request));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _destinationImageService.DeleteAsync(id);
        return NoContent();
    }

    /// <summary>Public like the destination itself, so the image URL works directly in any client image widget.</summary>
    [AllowAnonymous]
    [HttpGet("{id}/Content")]
    public async Task<IActionResult> GetContent(int id)
    {
        var (content, contentType) = await _destinationImageService.GetContentAsync(id);
        return File(content, contentType);
    }
}
