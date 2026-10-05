using TravelBay.Model.Exceptions;
using TravelBay.Model.Requests;
using TravelBay.Model.Responses;
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

    public async Task<List<SavedDestinationResponse>> GetAllAsync()
    {
        var userId = CurrentUserId();

        return await _dbContext.SavedDestinations
            .AsNoTracking()
            .Include(sd => sd.Destination)
            .Where(sd => sd.UserId == userId)
            .OrderByDescending(sd => sd.SavedAt)
            .Select(sd => new SavedDestinationResponse
            {
                Id = sd.Id,
                DestinationId = sd.DestinationId,
                DestinationName = sd.Destination.Name,
                SavedAt = sd.SavedAt
            })
            .ToListAsync();
    }

    public async Task<SavedDestinationResponse> AddAsync(SavedDestinationInsertRequest request)
    {
        var userId = CurrentUserId();

        var destination = await _dbContext.Destinations.FirstOrDefaultAsync(d => d.Id == request.DestinationId)
            ?? throw new ClientException("Destination not found.");

        var alreadySaved = await _dbContext.SavedDestinations
            .AnyAsync(sd => sd.UserId == userId && sd.DestinationId == request.DestinationId);
        if (alreadySaved)
        {
            throw new BusinessException("This destination is already saved.");
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
