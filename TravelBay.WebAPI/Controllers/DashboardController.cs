using TravelBay.Model.Constants;
using TravelBay.Model.Responses;
using TravelBay.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TravelBay.WebAPI.Controllers;

[Authorize(Roles = RoleNames.Admin)]
[ApiController]
[Route("[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("Stats")]
    public async Task<ActionResult<DashboardStatsResponse>> GetStats() => Ok(await _dashboardService.GetStatsAsync());
}
