using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;
using TravelBay.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TravelBay.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet]
    public async Task<ActionResult<PageResult<NotificationResponse>>> GetAll([FromQuery] NotificationSearchObject? search)
    {
        var result = await _notificationService.GetAllAsync(search);
        return Ok(result);
    }

    [HttpPut("{id}/MarkAsRead")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        await _notificationService.MarkAsReadAsync(id);
        return Ok(new { message = "Notifikacija označena kao pročitana." });
    }

    [HttpPut("MarkAllAsRead")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        await _notificationService.MarkAllAsReadAsync();
        return Ok(new { message = "Sve notifikacije označene kao pročitane." });
    }
}
