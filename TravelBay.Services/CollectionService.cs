using TravelBay.Model.Constants;
using TravelBay.Model.Exceptions;
using TravelBay.Model.Requests;
using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;
using TravelBay.Services.Database;
using FluentValidation;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace TravelBay.Services;

/// <summary>Strictly current-user owned — no Admin bypass for write (see docs/api-surface.md §4.3). Hard delete (no IsDeleted on Collection); EF cascades CollectionItems.</summary>
public class CollectionService
    : BaseCRUDService<Collection, CollectionResponse, CollectionSearchObject, CollectionInsertRequest, CollectionUpdateRequest>,
        ICollectionService
{
    private readonly IAuthenticatedUserAccessor _userAccessor;

    public CollectionService(
        TravelBayDbContext dbContext,
        IMapper mapper,
        IValidator<CollectionInsertRequest> insertValidator,
        IValidator<CollectionUpdateRequest> updateValidator,
        IAuthenticatedUserAccessor userAccessor)
        : base(dbContext, mapper, insertValidator, updateValidator)
    {
        _userAccessor = userAccessor;
    }

    private int CurrentUserId() =>
        _userAccessor.GetUserId() ?? throw new InvalidOperationException("User id claim is missing.");

    protected override IQueryable<Collection> ApplyFilters(IQueryable<Collection> query, CollectionSearchObject? search)
    {
        var isAdmin = _userAccessor.IsInRole(RoleNames.Admin);

        if (isAdmin && search?.UserId.HasValue == true)
        {
            query = query.Where(c => c.UserId == search.UserId.Value);
        }
        else if (!isAdmin)
        {
            query = query.Where(c => c.UserId == CurrentUserId());
        }

        return query;
    }

    protected override Task<IQueryable<Collection>> IncludeRelatedEntitiesAsync(CollectionSearchObject? search, IQueryable<Collection> query)
    {
        return base.IncludeRelatedEntitiesAsync(search, query.Include(c => c.Items));
    }

    public override async Task<CollectionResponse> GetByIdAsync(int id)
    {
        var entity = await GetOwnedEntityAsync(id, includeItems: true);
        return MapToResponse(entity);
    }

    public override async Task<CollectionResponse> InsertAsync(CollectionInsertRequest request)
    {
        await _insertValidator.ValidateAndThrowAsync(request);

        var entity = new Collection
        {
            UserId = CurrentUserId(),
            Name = request.Name,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Collections.Add(entity);
        await _dbContext.SaveChangesAsync();

        return MapToResponse(entity);
    }

    public override async Task<CollectionResponse> UpdateAsync(int id, CollectionUpdateRequest request)
    {
        await _updateValidator.ValidateAndThrowAsync(request);

        var entity = await GetOwnedEntityAsync(id, includeItems: true);
        entity.Name = request.Name;
        await _dbContext.SaveChangesAsync();

        return MapToResponse(entity);
    }

    public override async Task DeleteAsync(int id)
    {
        var entity = await GetOwnedEntityAsync(id, includeItems: false);
        _dbContext.Collections.Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<CollectionItemResponse>> GetItemsAsync(int collectionId)
    {
        await GetOwnedEntityAsync(collectionId, includeItems: false);

        return await _dbContext.CollectionItems
            .AsNoTracking()
            .Include(i => i.Destination)
            .Where(i => i.CollectionId == collectionId)
            .OrderByDescending(i => i.AddedAt)
            .Select(i => MapItemToResponse(i))
            .ToListAsync();
    }

    public async Task<CollectionItemResponse> AddItemAsync(int collectionId, CollectionItemInsertRequest request)
    {
        await GetOwnedEntityAsync(collectionId, includeItems: false);

        var destinationExists = await _dbContext.Destinations.AnyAsync(d => d.Id == request.DestinationId);
        if (!destinationExists)
        {
            throw new ClientException("Destination not found.");
        }

        var item = new CollectionItem
        {
            CollectionId = collectionId,
            DestinationId = request.DestinationId,
            AddedAt = DateTime.UtcNow
        };

        _dbContext.CollectionItems.Add(item);
        await _dbContext.SaveChangesAsync();

        var loaded = await _dbContext.CollectionItems.Include(i => i.Destination).FirstAsync(i => i.Id == item.Id);
        return MapItemToResponse(loaded);
    }

    public async Task RemoveItemAsync(int collectionId, int itemId)
    {
        await GetOwnedEntityAsync(collectionId, includeItems: false);

        var item = await _dbContext.CollectionItems.FirstOrDefaultAsync(i => i.Id == itemId && i.CollectionId == collectionId);
        if (item == null)
        {
            throw new NotFoundException($"CollectionItem with id {itemId} not found.");
        }

        _dbContext.CollectionItems.Remove(item);
        await _dbContext.SaveChangesAsync();
    }

    private async Task<Collection> GetOwnedEntityAsync(int collectionId, bool includeItems)
    {
        var userId = CurrentUserId();
        IQueryable<Collection> query = _dbContext.Collections;
        if (includeItems)
        {
            query = query.Include(c => c.Items).ThenInclude(i => i.Destination);
        }

        var entity = await query.FirstOrDefaultAsync(c => c.Id == collectionId);
        if (entity == null || entity.UserId != userId)
        {
            throw new NotFoundException($"Collection with id {collectionId} not found.");
        }

        return entity;
    }

    private static CollectionResponse MapToResponse(Collection entity) => new()
    {
        Id = entity.Id,
        UserId = entity.UserId,
        Name = entity.Name,
        CreatedAt = entity.CreatedAt,
        Items = entity.Items?.Select(MapItemToResponse).ToList() ?? new List<CollectionItemResponse>()
    };

    private static CollectionItemResponse MapItemToResponse(CollectionItem item) => new()
    {
        Id = item.Id,
        DestinationId = item.DestinationId,
        DestinationName = item.Destination?.Name ?? string.Empty,
        AddedAt = item.AddedAt
    };
}
