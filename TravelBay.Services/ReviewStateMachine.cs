using TravelBay.Model.Enums;
using TravelBay.Model.Exceptions;
using TravelBay.Model.Responses;
using TravelBay.Services.Database;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace TravelBay.Services;

/// <summary>Centralizes Review moderation (Pending -> Approved / Rejected). Admin-only (enforced at the controller).</summary>
public class ReviewStateMachine : IReviewStateMachine
{
    private readonly TravelBayDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IAuthenticatedUserAccessor _userAccessor;
    private readonly IAuditLogService _auditLogService;
    private readonly INotificationService _notificationService;

    public ReviewStateMachine(
        TravelBayDbContext dbContext,
        IMapper mapper,
        IAuthenticatedUserAccessor userAccessor,
        IAuditLogService auditLogService,
        INotificationService notificationService)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _userAccessor = userAccessor;
        _auditLogService = auditLogService;
        _notificationService = notificationService;
    }

    public async Task<ReviewResponse> ApproveAsync(int reviewId)
    {
        var review = await LoadPendingAsync(reviewId);
        var adminId = CurrentAdminId();

        review.Status = ReviewStatus.Approved;
        review.ModeratedByUserId = adminId;
        review.ModeratedAt = DateTime.UtcNow;
        review.ModerationReason = null;
        await _dbContext.SaveChangesAsync();

        await _auditLogService.LogAsync(nameof(Review), review.Id, "Approved", adminId, null);
        await _notificationService.CreateAsync(
            review.UserId, "Recenzija odobrena", "Vaša recenzija je odobrena i sada je javno vidljiva.", NotificationType.ReviewApproved);

        return _mapper.Map<ReviewResponse>(review);
    }

    public async Task<ReviewResponse> RejectAsync(int reviewId, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ClientException("A reason is required to reject a review.");
        }

        var review = await LoadPendingAsync(reviewId);
        var adminId = CurrentAdminId();

        review.Status = ReviewStatus.Rejected;
        review.ModeratedByUserId = adminId;
        review.ModeratedAt = DateTime.UtcNow;
        review.ModerationReason = reason.Trim();
        await _dbContext.SaveChangesAsync();

        await _auditLogService.LogAsync(nameof(Review), review.Id, "Rejected", adminId, review.ModerationReason);
        await _notificationService.CreateAsync(
            review.UserId, "Recenzija odbijena", $"Razlog: {review.ModerationReason}", NotificationType.ReviewRejected);

        return _mapper.Map<ReviewResponse>(review);
    }

    private async Task<Review> LoadPendingAsync(int reviewId)
    {
        var review = await _dbContext.Reviews.Include(r => r.User).FirstOrDefaultAsync(r => r.Id == reviewId)
            ?? throw new NotFoundException($"Review with id {reviewId} not found.");

        if (review.Status != ReviewStatus.Pending)
        {
            throw new BusinessException($"Review is already {review.Status} and cannot be moderated again.");
        }

        return review;
    }

    private int CurrentAdminId() =>
        _userAccessor.GetUserId() ?? throw new InvalidOperationException("User id claim is missing.");
}
