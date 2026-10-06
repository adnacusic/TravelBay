using TravelBay.Model.Constants;
using TravelBay.Model.SearchObjects;
using TravelBay.Services.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TravelBay.WebAPI.Controllers;

/// <summary>Admin PDF reports, generated on the server and returned as file downloads.</summary>
[Authorize(Roles = RoleNames.Admin)]
[ApiController]
[Route("[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("DestinationsByCategory")]
    [Produces(ReportFile.ContentType)]
    public async Task<IActionResult> DestinationsByCategory() =>
        AsFile(await _reportService.GetDestinationsByCategoryAsync());

    /// <summary>Optional period in whole days, e.g. ?from=2026-01-01&amp;to=2026-03-31.</summary>
    [HttpGet("UserActivity")]
    [Produces(ReportFile.ContentType)]
    public async Task<IActionResult> UserActivity([FromQuery] UserActivityReportFilter filter) =>
        AsFile(await _reportService.GetUserActivityAsync(filter));

    /// <summary>Optional ?top=N (1–50, default 10).</summary>
    [HttpGet("PopularDestinations")]
    [Produces(ReportFile.ContentType)]
    public async Task<IActionResult> PopularDestinations([FromQuery] PopularDestinationsReportFilter filter) =>
        AsFile(await _reportService.GetPopularDestinationsAsync(filter));

    private FileContentResult AsFile(ReportFile report) => File(report.Content, ReportFile.ContentType, report.FileName);
}
