using TravelBay.Model.Constants;
using TravelBay.Model.Exceptions;
using TravelBay.Model.Requests;
using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;
using TravelBay.Services.Database;
using Microsoft.EntityFrameworkCore;

namespace TravelBay.Services;

public class SavedDestinationService : ISavedDestinationService
{
    private readonly TravelBayDbContext _dbContext;
    private readonly IAuthenticatedUserAccessor _userAccessor;

    public SavedDestinationService(TravelBayDbContext dbContext, IAuthenticatedUserAccessor userAccessor)
    {
        _dbContext = dbContext;
        _userAccessor = userAccessor;
    }

    private int CurrentUserId() =>
        _userAccessor.GetUserId() ?? throw new InvalidOperationException("User id claim is missing.");

    public async Task<PageResult<SavedDestinationResponse>> GetAllAsync(SavedDestinationSearchObject? search)
    {
        search ??= new SavedDestinationSearchObject();
        var userId = CurrentUserId();

        var query = _dbContext.SavedDestinations
            .AsNoTracking()
            .Where(sd => sd.UserId == userId);

        if (search.DestinationId.HasValue)
        {
            query = query.Where(sd => sd.DestinationId == search.DestinationId.Value);
        }

        int? totalCount = search.IncludeTotalCount == true ? await query.CountAsync() : null;
        var page = search.Page ?? 1;
        var pageSize = search.PageSize ?? PagingDefaults.DefaultPageSize;

        var items = await query
            .OrderByDescending(sd => sd.SavedAt)
            .ThenByDescending(sd => sd.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(sd => new SavedDestinationResponse
            {
                Id = sd.Id,
                DestinationId = sd.DestinationId,
                DestinationName = sd.Destination.Name,
                CityName = sd.Destination.City.Name,
                ImageUrl = sd.Destination.Images.OrderBy(i => i.OrderIndex).Select(i => i.ImageUrl).FirstOrDefault(),
                SavedAt = sd.SavedAt
            })
            .ToListAsync();

        return new PageResult<SavedDestinationResponse> { Items = items, TotalCount = totalCount };
    }

    public async Task<SavedDestinationResponse> AddAsync(SavedDestinationInsertRequest request)
    {
        var userId = CurrentUserId();

        var destination = await _dbContext.Destinations
            .Include(d => d.City)
            .Include(d => d.Images)
            .FirstOrDefaultAsync(d => d.Id == request.DestinationId)
            ?? throw new ClientException("Destination not found.");

        var alreadySaved = await _dbContext.SavedDestinations
            .AnyAsync(sd => sd.UserId == userId && sd.DestinationId == request.DestinationId);
        if (alreadySaved)
        {
            throw new BusinessException("Ova destinacija je već sačuvana.");
        }

        var entity = new SavedDestination
        {
            UserId = userId,
            DestinationId = request.DestinationId,
            SavedAt = DateTime.UtcNow
        };

        _dbContext.SavedDestinations.Add(entity);
        await _dbContext.SaveChangesAsync();

        return new SavedDestinationResponse
        {
            Id = entity.Id,
            DestinationId = entity.DestinationId,
            DestinationName = destination.Name,
            CityName = destination.City.Name,
            ImageUrl = destination.Images.OrderBy(i => i.OrderIndex).Select(i => i.ImageUrl).FirstOrDefault(),
            SavedAt = entity.SavedAt
        };
    }

    public async Task RemoveAsync(int id)
    {
        var userId = CurrentUserId();

        var entity = await _dbContext.SavedDestinations.FirstOrDefaultAsync(sd => sd.Id == id);
        if (entity == null || entity.UserId != userId)
        {
            throw new NotFoundException($"SavedDestination with id {id} not found.");
        }

        _dbContext.SavedDestinations.Remove(entity);
        await _dbContext.SaveChangesAsync();
    }
}
