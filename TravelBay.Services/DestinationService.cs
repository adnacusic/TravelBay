using System;
using System.Collections.Generic;
using System.Linq;
using TravelBay.Model.Enums;
using TravelBay.Model.Exceptions;
using TravelBay.Model.Requests;
using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;
using TravelBay.Services.Database;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace TravelBay.Services;

public class DestinationService
    : BaseCRUDService<Destination, DestinationResponse, DestinationSearchObject, DestinationInsertRequest, DestinationUpdateRequest>,
        IDestinationService
{
    public DestinationService(
        TravelBayDbContext dbContext,
        MapsterMapper.IMapper mapper,
        IValidator<DestinationInsertRequest> insertValidator,
        IValidator<DestinationUpdateRequest> updateValidator)
        : base(dbContext, mapper, insertValidator, updateValidator)
    {
    }

    protected override Task<IQueryable<Destination>> IncludeRelatedEntitiesAsync(DestinationSearchObject? search, IQueryable<Destination> query = null!)
    {
        // City name is part of every destination response (lists show it without a second lookup).
        query = query.Include(d => d.City);

        if (search?.IncludeCategory == true)
        {
            query = query.Include(d => d.Category);
        }
        if (search?.IncludeImages == true)
        {
            query = query.Include(d => d.Images);
        }
        return base.IncludeRelatedEntitiesAsync(search, query);
    }

    protected override IQueryable<Destination> ApplyFilters(IQueryable<Destination> query, DestinationSearchObject? search)
    {
        if (search != null)
        {
            if (!string.IsNullOrWhiteSpace(search.Name))
            {
                query = query.Where(d => d.Name.Contains(search.Name));
            }
            if (!string.IsNullOrWhiteSpace(search.Description))
            {
                query = query.Where(d => d.Description.Contains(search.Description));
            }
            if (search.CategoryId.HasValue)
            {
                query = query.Where(d => d.CategoryId == search.CategoryId.Value);
            }
            if (search.CityId.HasValue)
            {
                query = query.Where(d => d.CityId == search.CityId.Value);
            }
        }

        return query;
    }

    public const int DefaultPopularCount = 10;
    public const int MaxPopularCount = 50;

    /// <summary>List items also carry the approved-review count and average (one grouped query per page).</summary>
    public override async Task<PageResult<DestinationResponse>> GetAllAsync(DestinationSearchObject? search = null)
    {
        var result = await base.GetAllAsync(search);
        await FillRatingsAsync(result.Items);
        return result;
    }

    public async Task<PageResult<DestinationResponse>> GetPopularAsync(int? top)
    {
        var count = Math.Clamp(top ?? DefaultPopularCount, 1, MaxPopularCount);

        // Ranking runs in SQL; only the ids of the top N come back.
        var rankedIds = await _dbContext.Destinations
            .AsNoTracking()
            .Select(d => new
            {
                d.Id,
                d.Name,
                ViewCount = d.ViewHistoryEntries.Count(),
                ApprovedReviewCount = d.Reviews.Count(r => r.Status == ReviewStatus.Approved),
                AverageRating = d.Reviews
                    .Where(r => r.Status == ReviewStatus.Approved)
                    .Average(r => (double?)r.Rating)
            })
            .OrderByDescending(d => d.ViewCount)
            .ThenByDescending(d => d.ApprovedReviewCount)
            .ThenByDescending(d => d.AverageRating)
            .ThenBy(d => d.Name)
            .Take(count)
            .Select(d => d.Id)
            .ToListAsync();

        var entities = await _dbContext.Destinations
            .AsNoTracking()
            .Include(d => d.Category)
            .Include(d => d.City)
            .Include(d => d.Images)
            .Where(d => rankedIds.Contains(d.Id))
            .ToDictionaryAsync(d => d.Id);

        var items = rankedIds.Select(id => _mapper.Map<DestinationResponse>(entities[id])).ToList();
        await FillRatingsAsync(items);

        return new PageResult<DestinationResponse> { Items = items, TotalCount = items.Count };
    }

    private async Task FillRatingsAsync(List<DestinationResponse> items)
    {
        var ids = items.Select(d => d.Id).ToList();
        var ratings = await _dbContext.Reviews
            .AsNoTracking()
            .Where(r => ids.Contains(r.DestinationId) && r.Status == ReviewStatus.Approved)
            .GroupBy(r => r.DestinationId)
            .Select(g => new { DestinationId = g.Key, Count = g.Count(), Average = g.Average(r => (double)r.Rating) })
            .ToDictionaryAsync(x => x.DestinationId);

        foreach (var item in items)
        {
            if (ratings.TryGetValue(item.Id, out var rating))
            {
                item.ReviewCount = rating.Count;
                item.AverageRating = rating.Average;
            }
        }
    }

    public override async Task<DestinationResponse> GetByIdAsync(int id)
    {
        var entity = await _dbContext.Destinations
            .AsNoTracking()
            .Include(d => d.Category)
            .Include(d => d.City)
            .Include(d => d.Images)
            .Include(d => d.Reviews.Where(r => r.Status == ReviewStatus.Approved))
            .FirstOrDefaultAsync(d => d.Id == id);

        if (entity == null)
        {
            throw new NotFoundException($"Destination with id {id} not found.");
        }

        var response = _mapper.Map<DestinationResponse>(entity);
        response.ReviewCount = entity.Reviews.Count;
        response.AverageRating = entity.Reviews.Count > 0 ? entity.Reviews.Average(r => r.Rating) : null;

        return response;
    }

    /// <summary>Business entities use soft delete (status flip), never a hard SQL DELETE.</summary>
    public override async Task DeleteAsync(int id)
    {
        var entity = await _dbContext.Destinations.FindAsync(id);
        if (entity == null)
        {
            throw new NotFoundException($"Destination with id {id} not found.");
        }

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
    }
}
