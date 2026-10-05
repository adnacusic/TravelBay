using TravelBay.Model.Requests;
using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;
using TravelBay.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TravelBay.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class ViewHistoriesController : ControllerBase
{
    private readonly IViewHistoryService _viewHistoryService;

    public ViewHistoriesController(IViewHistoryService viewHistoryService)
    {
        _viewHistoryService = viewHistoryService;
    }

    [HttpPost]
    public async Task<ActionResult<ViewHistoryResponse>> Record([FromBody] ViewHistoryInsertRequest request) =>
        Ok(await _viewHistoryService.RecordAsync(request));

    [HttpGet]
    public async Task<ActionResult<PageResult<ViewHistoryResponse>>> GetAll([FromQuery] ViewHistorySearchObject? search) =>
        Ok(await _viewHistoryService.GetAllAsync(search));
}
