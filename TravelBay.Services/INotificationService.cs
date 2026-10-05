using TravelBay.Model.Enums;
using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;

namespace TravelBay.Services;

/// <summary>Current-user scoped: every method implicitly operates on the caller's own notifications.</summary>
public interface INotificationService
{
    Task<PageResult<NotificationResponse>> GetAllAsync(NotificationSearchObject? search = null);
    Task MarkAsReadAsync(int id);
    Task MarkAllAsReadAsync();

    /// <summary>Internal write hook used by other services (Review/TripPlan state machines) — there is no public create endpoint.</summary>
    Task CreateAsync(int userId, string title, string message, NotificationType type);
}
