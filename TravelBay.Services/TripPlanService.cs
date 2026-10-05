using TravelBay.Model.Exceptions;
using TravelBay.Model.Requests;
using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;
using TravelBay.Services.Database;
using FluentValidation;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace TravelBay.Services;

/// <summary>Strictly current-user owned — there is no Admin bypass for write operations here (see docs/api-surface.md).</summary>
public class TripPlanService
    : BaseCRUDService<TripPlan, TripPlanResponse, TripPlanSearchObject, TripPlanInsertRequest, TripPlanUpdateRequest>,
        ITripPlanService
{
    private readonly IAuthenticatedUserAccessor _userAccessor;

    public TripPlanService(
        TravelBayDbContext dbContext,
        IMapper mapper,
        IValidator<TripPlanInsertRequest> insertValidator,
        IValidator<TripPlanUpdateRequest> updateValidator,
        IAuthenticatedUserAccessor userAccessor)
        : base(dbContext, mapper, insertValidator, updateValidator)
    {
        _userAccessor = userAccessor;
    }

    private int CurrentUserId() =>
        _userAccessor.GetUserId() ?? throw new InvalidOperationException("User id claim is missing.");

    protected override IQueryable<TripPlan> ApplyFilters(IQueryable<TripPlan> query, TripPlanSearchObject? search)
    {
        var userId = CurrentUserId();
        var isAdmin = _userAccessor.IsInRole(Model.Constants.RoleNames.Admin);

        if (isAdmin && search?.UserId.HasValue == true)
        {
            query = query.Where(tp => tp.UserId == search.UserId.Value);
        }
        else if (!isAdmin)
        {
            query = query.Where(tp => tp.UserId == userId);
        }

        if (search?.Status.HasValue == true)
        {
            query = query.Where(tp => tp.Status == search.Status.Value);
        }

        return query;
    }

    protected override Task<IQueryable<TripPlan>> IncludeRelatedEntitiesAsync(TripPlanSearchObject? search, IQueryable<TripPlan> query)
    {
        return base.IncludeRelatedEntitiesAsync(search, query.Include(tp => tp.Items));
    }

    public override async Task<TripPlanResponse> GetByIdAsync(int id)
    {
        var userId = CurrentUserId();
        var isAdmin = _userAccessor.IsInRole(Model.Constants.RoleNames.Admin);

        var entity = await _dbContext.TripPlans
            .AsNoTracking()
            .Include(tp => tp.Items).ThenInclude(i => i.Destination)
            .FirstOrDefaultAsync(tp => tp.Id == id);

        if (entity == null || (!isAdmin && entity.UserId != userId))
        {
            throw new NotFoundException($"TripPlan with id {id} not found.");
        }

        return MapToResponse(entity);
    }

    public override async Task<TripPlanResponse> InsertAsync(TripPlanInsertRequest request)
    {
        await _insertValidator.ValidateAndThrowAsync(request);

        var entity = new TripPlan
        {
            UserId = CurrentUserId(),
            Name = request.Name,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Status = Model.Enums.TripPlanStatus.Draft,
            CreatedAt = DateTime.UtcNow,
        };

        _dbContext.TripPlans.Add(entity);
        await _dbContext.SaveChangesAsync();

        return MapToResponse(entity);
    }

    public override async Task<TripPlanResponse> UpdateAsync(int id, TripPlanUpdateRequest request)
    {
        await _updateValidator.ValidateAndThrowAsync(request);

        var entity = await GetOwnedEntityAsync(id);

        entity.Name = request.Name;
        entity.StartDate = request.StartDate;
        entity.EndDate = request.EndDate;
        entity.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        return MapToResponse(entity);
    }

    /// <summary>Business entities use soft delete (status flip), never a hard SQL DELETE.</summary>
    public override async Task DeleteAsync(int id)
    {
        var entity = await GetOwnedEntityAsync(id);
        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<TripPlanItemResponse>> GetItemsAsync(int tripPlanId)
    {
        await GetOwnedEntityAsync(tripPlanId);

        return await _dbContext.TripPlanItems
            .AsNoTracking()
            .Include(i => i.Destination)
            .Where(i => i.TripPlanId == tripPlanId)
            .OrderBy(i => i.DayNumber).ThenBy(i => i.OrderIndex)
            .Select(i => MapItemToResponse(i))
            .ToListAsync();
    }

    public async Task<TripPlanItemResponse> AddItemAsync(int tripPlanId, TripPlanItemInsertRequest request)
    {
        await GetOwnedEntityAsync(tripPlanId);

        var destinationExists = await _dbContext.Destinations.AnyAsync(d => d.Id == request.DestinationId);
        if (!destinationExists)
        {
            throw new ClientException("Destination not found.");
        }

        var item = new TripPlanItem
        {
            TripPlanId = tripPlanId,
            DestinationId = request.DestinationId,
            DayNumber = request.DayNumber,
            OrderIndex = request.OrderIndex,
            Notes = request.Notes
        };

        _dbContext.TripPlanItems.Add(item);
        await _dbContext.SaveChangesAsync();

        var loaded = await _dbContext.TripPlanItems.Include(i => i.Destination).FirstAsync(i => i.Id == item.Id);
        return MapItemToResponse(loaded);
    }

    public async Task<TripPlanItemResponse> UpdateItemAsync(int tripPlanId, int itemId, TripPlanItemUpdateRequest request)
    {
        await GetOwnedEntityAsync(tripPlanId);

        var item = await _dbContext.TripPlanItems.Include(i => i.Destination)
            .FirstOrDefaultAsync(i => i.Id == itemId && i.TripPlanId == tripPlanId);
        if (item == null)
        {
            throw new NotFoundException($"TripPlanItem with id {itemId} not found.");
        }

        item.DayNumber = request.DayNumber;
        item.OrderIndex = request.OrderIndex;
        item.Notes = request.Notes;
        await _dbContext.SaveChangesAsync();

        return MapItemToResponse(item);
    }

    public async Task RemoveItemAsync(int tripPlanId, int itemId)
    {
        await GetOwnedEntityAsync(tripPlanId);

        var item = await _dbContext.TripPlanItems.FirstOrDefaultAsync(i => i.Id == itemId && i.TripPlanId == tripPlanId);
        if (item == null)
        {
            throw new NotFoundException($"TripPlanItem with id {itemId} not found.");
        }

        _dbContext.TripPlanItems.Remove(item);
        await _dbContext.SaveChangesAsync();
    }

    private async Task<TripPlan> GetOwnedEntityAsync(int tripPlanId)
    {
        var userId = CurrentUserId();
        var entity = await _dbContext.TripPlans.FirstOrDefaultAsync(tp => tp.Id == tripPlanId);
        if (entity == null || entity.UserId != userId)
        {
            throw new NotFoundException($"TripPlan with id {tripPlanId} not found.");
        }

        return entity;
    }

    private static TripPlanResponse MapToResponse(TripPlan entity) => new()
    {
        Id = entity.Id,
        UserId = entity.UserId,
        Name = entity.Name,
        StartDate = entity.StartDate,
        EndDate = entity.EndDate,
        Status = entity.Status,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt,
        Items = entity.Items?.Select(MapItemToResponse).ToList() ?? new List<TripPlanItemResponse>()
    };

    private static TripPlanItemResponse MapItemToResponse(TripPlanItem item) => new()
    {
        Id = item.Id,
        DestinationId = item.DestinationId,
        DestinationName = item.Destination?.Name ?? string.Empty,
        DayNumber = item.DayNumber,
        OrderIndex = item.OrderIndex,
        Notes = item.Notes
    };
}
