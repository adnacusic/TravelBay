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
        _auditLogService.Log(nameof(Review), review.Id, "Approved", adminId, null);
        _notificationService.Add(
            review.UserId, "Recenzija odobrena", $"Vaša recenzija destinacije {review.Destination.Name} je odobrena i sada je javno vidljiva.", NotificationType.ReviewApproved);
        await _dbContext.SaveChangesAsync();

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
        _auditLogService.Log(nameof(Review), review.Id, "Rejected", adminId, review.ModerationReason);
        _notificationService.Add(
            review.UserId, "Recenzija odbijena", $"Vaša recenzija destinacije {review.Destination.Name} nije objavljena. Razlog: {review.ModerationReason}", NotificationType.ReviewRejected);
        await _dbContext.SaveChangesAsync();

        return _mapper.Map<ReviewResponse>(review);
    }

    public void PrepareForEdit(Review review, int authorId)
    {
        switch (review.Status)
        {
            case ReviewStatus.Pending:
                return;
            case ReviewStatus.Approved:
                review.Status = ReviewStatus.Pending;
                review.ModeratedByUserId = null;
                review.ModeratedAt = null;
                review.ModerationReason = null;
                _auditLogService.Log(nameof(Review), review.Id, "ResubmittedAfterEdit", authorId, "Approved review edited by its author; back to Pending.");
                return;
            default:
                throw new BusinessException("Odbijena recenzija se ne može mijenjati. Napišite novu recenziju.");
        }
    }

    private async Task<Review> LoadPendingAsync(int reviewId)
    {
        var review = await _dbContext.Reviews
            .Include(r => r.User)
            .Include(r => r.Destination)
            .Include(r => r.ModeratedByUser)
            .FirstOrDefaultAsync(r => r.Id == reviewId)
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
