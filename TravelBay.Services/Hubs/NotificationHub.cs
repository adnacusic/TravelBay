using TravelBay.Model.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace TravelBay.Services.Hubs;

/// <summary>
/// Pushes notifications to the user they belong to. Each connection joins a per-user group
/// (identity taken from the JWT, never from client input) so a user only ever receives their own.
/// </summary>
[Authorize]
public class NotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst(ClaimNames.Id)?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(userId));
        }

        await base.OnConnectedAsync();
    }

    public static string GroupName(string userId) => $"user-{userId}";
}
