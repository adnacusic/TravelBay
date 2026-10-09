using TravelBay.Model.Enums;
using TravelBay.Model.Exceptions;
using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;
using TravelBay.Services.Database;
using Microsoft.EntityFrameworkCore;

namespace TravelBay.Services;

public class NotificationService : INotificationService
{
    private readonly TravelBayDbContext _dbContext;
    private readonly IAuthenticatedUserAccessor _userAccessor;

    public NotificationService(TravelBayDbContext dbContext, IAuthenticatedUserAccessor userAccessor)
    {
        _dbContext = dbContext;
        _userAccessor = userAccessor;
    }

    private int CurrentUserId() =>
        _userAccessor.GetUserId() ?? throw new InvalidOperationException("User id claim is missing.");

    public async Task<PageResult<NotificationResponse>> GetAllAsync(NotificationSearchObject? search = null)
    {
        search ??= new NotificationSearchObject();
        var userId = CurrentUserId();

        IQueryable<Notification> query = _dbContext.Notifications
            .AsNoTracking()
            .Where(n => n.UserId == userId);

        if (search.IsRead.HasValue)
        {
            query = query.Where(n => n.IsRead == search.IsRead.Value);
        }

        query = query.OrderByDescending(n => n.CreatedAt);

        int? totalCount = search.IncludeTotalCount == true ? await query.CountAsync() : null;

        if (search.Page.HasValue)
        {
            query = query.Skip((search.Page.Value - 1) * (search.PageSize ?? 10));
        }
        if (search.PageSize.HasValue)
        {
            query = query.Take(search.PageSize.Value);
        }

        var items = await query
            .Select(n => new NotificationResponse
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Message,
                Type = n.Type,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt
            })
            .ToListAsync();

        return new PageResult<NotificationResponse> { Items = items, TotalCount = totalCount };
    }

    public async Task MarkAsReadAsync(int id)
    {
        var userId = CurrentUserId();
        var entity = await _dbContext.Notifications.FirstOrDefaultAsync(n => n.Id == id);
        if (entity == null || entity.UserId != userId)
        {
            throw new NotFoundException($"Notification with id {id} not found.");
        }

        entity.IsRead = true;
        await _dbContext.SaveChangesAsync();
    }

    public async Task MarkAllAsReadAsync()
    {
        var userId = CurrentUserId();
        await _dbContext.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ExecuteUpdateAsync(setters => setters.SetProperty(n => n.IsRead, true));
    }

    public void Add(int userId, string title, string message, NotificationType type)
    {
        _dbContext.Notifications.Add(new Notification
        {
            UserId = userId,
            Title = title,
            Message = message,
            Type = type,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        });
    }
}
