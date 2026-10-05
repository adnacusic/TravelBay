using TravelBay.Model.Enums;
using TravelBay.Model.Exceptions;
using TravelBay.Model.Responses;
using TravelBay.Services.Database;
using Microsoft.EntityFrameworkCore;

namespace TravelBay.Services;

/// <summary>
/// Centralizes TripPlan status transitions (Draft -> Active -> Completed / Cancelled). Owner-only —
/// unlike Review moderation, Admin has no bypass here (see docs/api-surface.md §4.3).
/// </summary>
public class TripPlanStateMachine : ITripPlanStateMachine
{
    private readonly TravelBayDbContext _dbContext;
    private readonly IAuthenticatedUserAccessor _userAccessor;
    private readonly IAuditLogService _auditLogService;
    private readonly INotificationService _notificationService;

    public TripPlanStateMachine(
        TravelBayDbContext dbContext,
        IAuthenticatedUserAccessor userAccessor,
        IAuditLogService auditLogService,
        INotificationService notificationService)
    {
        _dbContext = dbContext;
        _userAccessor = userAccessor;
        _auditLogService = auditLogService;
        _notificationService = notificationService;
    }

    public Task<TripPlanResponse> ActivateAsync(int tripPlanId) =>
        TransitionAsync(tripPlanId, from: TripPlanStatus.Draft, to: TripPlanStatus.Active);

    public Task<TripPlanResponse> CompleteAsync(int tripPlanId) =>
        TransitionAsync(tripPlanId, from: TripPlanStatus.Active, to: TripPlanStatus.Completed);

    public Task<TripPlanResponse> CancelAsync(int tripPlanId) =>
        TransitionAsync(tripPlanId, from: null, to: TripPlanStatus.Cancelled, allowedFrom: new[] { TripPlanStatus.Draft, TripPlanStatus.Active });

    private async Task<TripPlanResponse> TransitionAsync(
        int tripPlanId,
        TripPlanStatus? from,
        TripPlanStatus to,
        TripPlanStatus[]? allowedFrom = null)
    {
        var userId = _userAccessor.GetUserId() ?? throw new InvalidOperationException("User id claim is missing.");

        var entity = await _dbContext.TripPlans
            .Include(tp => tp.Items).ThenInclude(i => i.Destination)
            .FirstOrDefaultAsync(tp => tp.Id == tripPlanId);

        if (entity == null || entity.UserId != userId)
        {
            throw new NotFoundException($"TripPlan with id {tripPlanId} not found.");
        }

        var allowed = allowedFrom ?? new[] { from!.Value };
        if (!allowed.Contains(entity.Status))
        {
            throw new BusinessException($"Cannot move TripPlan from {entity.Status} to {to}.");
        }

        entity.Status = to;
        entity.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        await _auditLogService.LogAsync(nameof(TripPlan), entity.Id, $"StatusChanged:{to}", userId, $"Trip plan '{entity.Name}' moved to {to}.");
        await _notificationService.CreateAsync(entity.UserId, "Status plana putovanja promijenjen", $"Plan '{entity.Name}' je sada {to}.", NotificationType.TripStatusChanged);

        return new TripPlanResponse
        {
            Id = entity.Id,
            UserId = entity.UserId,
            Name = entity.Name,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            Status = entity.Status,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            Items = entity.Items.Select(i => new TripPlanItemResponse
            {
                Id = i.Id,
                DestinationId = i.DestinationId,
                DestinationName = i.Destination?.Name ?? string.Empty,
                DayNumber = i.DayNumber,
                OrderIndex = i.OrderIndex,
                Notes = i.Notes
            }).ToList()
        };
    }
}
