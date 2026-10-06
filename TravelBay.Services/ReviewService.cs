using System;
using System.Collections.Generic;
using System.Linq;
using TravelBay.Model.Constants;
using TravelBay.Model.Enums;
using TravelBay.Model.Exceptions;
using TravelBay.Model.Requests;
using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;
using TravelBay.Services.Database;
using FluentValidation;
using FluentValidation.Results;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace TravelBay.Services;

public class ReviewService
    : BaseCRUDService<Review, ReviewResponse, ReviewSearchObject, ReviewInsertRequest, ReviewUpdateRequest>,
        IReviewService
{
    private readonly IAuthenticatedUserAccessor _userAccessor;

    public ReviewService(
        TravelBayDbContext dbContext,
        IMapper mapper,
        IValidator<ReviewInsertRequest> insertValidator,
        IValidator<ReviewUpdateRequest> updateValidator,
        IAuthenticatedUserAccessor userAccessor)
        : base(dbContext, mapper, insertValidator, updateValidator)
    {
        _userAccessor = userAccessor;
    }

    public override async Task<PageResult<ReviewResponse>> GetAllAsync(ReviewSearchObject? search = null)
    {
        search ??= new ReviewSearchObject();
        if (string.IsNullOrWhiteSpace(search.SortBy))
        {
            search.SortBy = "CreatedAt desc";
        }

        return await base.GetAllAsync(search);
    }

    protected override async Task<IQueryable<Review>> IncludeRelatedEntitiesAsync(ReviewSearchObject? search, IQueryable<Review> query = null!)
    {
        return await Task.FromResult(query
            .Include(r => r.User)
            .Include(r => r.Destination)
            .Include(r => r.ModeratedByUser));
    }

    protected override IQueryable<Review> ApplyFilters(IQueryable<Review> query, ReviewSearchObject? search)
    {
        if (_userAccessor.IsInRole(RoleNames.Admin))
        {
            if (search?.UserId.HasValue == true)
            {
                query = query.Where(r => r.UserId == search.UserId.Value);
            }
            if (search?.Status.HasValue == true)
            {
                query = query.Where(r => r.Status == search.Status.Value);
            }
        }
        else
        {
            // Public visibility: approved reviews from anyone, plus the caller's own (any status).
            var userId = _userAccessor.GetUserId();
            query = query.Where(r => r.Status == ReviewStatus.Approved || (userId.HasValue && r.UserId == userId.Value));
        }

        if (search?.DestinationId.HasValue == true)
        {
            query = query.Where(r => r.DestinationId == search.DestinationId.Value);
        }

        if (search?.Rating.HasValue == true)
        {
            query = query.Where(r => r.Rating == search.Rating.Value);
        }

        return query;
    }

    public override async Task<ReviewResponse> InsertAsync(ReviewInsertRequest request)
    {
        var userId = _userAccessor.GetUserId()
                     ?? throw new InvalidOperationException("User id claim is missing.");

        var validationResult = await _insertValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => _mapper.Map<ValidationFailure>(e));
            throw new FluentValidation.ValidationException(errors);
        }

        var destinationExists = await _dbContext.Destinations.AnyAsync(d => d.Id == request.DestinationId);
        if (!destinationExists)
        {
            throw new ClientException("Destination not found.");
        }

        var review = new Review
        {
            UserId = userId,
            DestinationId = request.DestinationId,
            Rating = request.Rating,
            Comment = request.Comment?.Trim(),
            Status = ReviewStatus.Pending,
            CreatedAt = DateTime.UtcNow,
        };

        _dbContext.Reviews.Add(review);
        await _dbContext.SaveChangesAsync();

        var loaded = await _dbContext.Reviews
            .AsNoTracking()
            .Include(r => r.User)
            .Include(r => r.Destination)
            .Include(r => r.ModeratedByUser)
            .FirstAsync(r => r.Id == review.Id);

        return _mapper.Map<ReviewResponse>(loaded);
    }

    public override async Task<ReviewResponse> UpdateAsync(int id, ReviewUpdateRequest request)
    {
        var userId = _userAccessor.GetUserId()
                     ?? throw new InvalidOperationException("User id claim is missing.");

        var validationResult = await _updateValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => _mapper.Map<ValidationFailure>(e));
            throw new FluentValidation.ValidationException(errors);
        }

        var entity = await _dbContext.Reviews.FindAsync(id);
        if (entity == null || entity.UserId != userId)
        {
            throw new NotFoundException($"{nameof(Review)} with id {id} not found.");
        }

        entity.Rating = request.Rating;
        entity.Comment = request.Comment?.Trim();
        await _dbContext.SaveChangesAsync();

        var loaded = await _dbContext.Reviews
            .AsNoTracking()
            .Include(r => r.User)
            .Include(r => r.Destination)
            .Include(r => r.ModeratedByUser)
            .FirstAsync(r => r.Id == id);

        return _mapper.Map<ReviewResponse>(loaded);
    }

    /// <summary>
    /// Business entities use soft delete (status flip), never a hard SQL DELETE. Owners delete
    /// their own review; Admins may also delete any review as part of moderation.
    /// </summary>
    public override async Task DeleteAsync(int id)
    {
        var userId = _userAccessor.GetUserId()
                     ?? throw new InvalidOperationException("User id claim is missing.");
        var isAdmin = _userAccessor.IsInRole(RoleNames.Admin);

        var entity = await _dbContext.Reviews.FindAsync(id);
        if (entity == null || (!isAdmin && entity.UserId != userId))
        {
            throw new NotFoundException($"{nameof(Review)} with id {id} not found.");
        }

        entity.IsDeleted = true;
        await _dbContext.SaveChangesAsync();
    }
}
