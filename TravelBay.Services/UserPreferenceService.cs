using TravelBay.Model.Exceptions;
using TravelBay.Model.Requests;
using TravelBay.Model.Responses;
using TravelBay.Services.Database;
using Microsoft.EntityFrameworkCore;

namespace TravelBay.Services;

public class UserPreferenceService : IUserPreferenceService
{
    private readonly TravelBayDbContext _dbContext;
    private readonly IAuthenticatedUserAccessor _userAccessor;

    public UserPreferenceService(TravelBayDbContext dbContext, IAuthenticatedUserAccessor userAccessor)
    {
        _dbContext = dbContext;
        _userAccessor = userAccessor;
    }

    private int CurrentUserId() =>
        _userAccessor.GetUserId() ?? throw new InvalidOperationException("User id claim is missing.");

    public async Task<List<UserPreferenceResponse>> GetAllAsync()
    {
        var userId = CurrentUserId();

        return await _dbContext.UserPreferences
            .AsNoTracking()
            .Include(up => up.Category)
            .Where(up => up.UserId == userId)
            .Select(up => new UserPreferenceResponse { CategoryId = up.CategoryId, CategoryName = up.Category.Name })
            .ToListAsync();
    }

    public async Task<List<UserPreferenceResponse>> SetAsync(SetUserPreferencesRequest request)
    {
        var userId = CurrentUserId();
        var distinctCategoryIds = request.CategoryIds.Distinct().ToList();

        if (distinctCategoryIds.Count > 0)
        {
            var validCount = await _dbContext.Categories.CountAsync(c => distinctCategoryIds.Contains(c.Id));
            if (validCount != distinctCategoryIds.Count)
            {
                throw new ClientException("One or more category ids do not exist.");
            }
        }

        var existing = _dbContext.UserPreferences.Where(up => up.UserId == userId);
        _dbContext.UserPreferences.RemoveRange(existing);

        foreach (var categoryId in distinctCategoryIds)
        {
            _dbContext.UserPreferences.Add(new UserPreference { UserId = userId, CategoryId = categoryId });
        }

        await _dbContext.SaveChangesAsync();

        return await GetAllAsync();
    }
}
