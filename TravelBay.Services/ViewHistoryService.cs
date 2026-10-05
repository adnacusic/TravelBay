using TravelBay.Model.Exceptions;
using TravelBay.Model.Requests;
using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;
using TravelBay.Services.Database;
using Microsoft.EntityFrameworkCore;

namespace TravelBay.Services;

public class ViewHistoryService : IViewHistoryService
{
    private readonly TravelBayDbContext _dbContext;
    private readonly IAuthenticatedUserAccessor _userAccessor;

    public ViewHistoryService(TravelBayDbContext dbContext, IAuthenticatedUserAccessor userAccessor)
    {
        _dbContext = dbContext;
        _userAccessor = userAccessor;
    }

    private int CurrentUserId() =>
        _userAccessor.GetUserId() ?? throw new InvalidOperationException("User id claim is missing.");

    public async Task<ViewHistoryResponse> RecordAsync(ViewHistoryInsertRequest request)
    {
        var userId = CurrentUserId();

        var destination = await _dbContext.Destinations.FirstOrDefaultAsync(d => d.Id == request.DestinationId)
            ?? throw new ClientException("Destination not found.");

        var entity = new ViewHistory
        {
            UserId = userId,
            DestinationId = request.DestinationId,
            ViewedAt = DateTime.UtcNow
        };

        _dbContext.ViewHistories.Add(entity);
        await _dbContext.SaveChangesAsync();

        return new ViewHistoryResponse
        {
            Id = entity.Id,
            DestinationId = entity.DestinationId,
            DestinationName = destination.Name,
            ViewedAt = entity.ViewedAt
        };
    }

    public async Task<PageResult<ViewHistoryResponse>> GetAllAsync(ViewHistorySearchObject? search = null)
    {
        search ??= new ViewHistorySearchObject();
        var userId = CurrentUserId();

        IQueryable<ViewHistory> query = _dbContext.ViewHistories
            .AsNoTracking()
            .Include(vh => vh.Destination)
            .Where(vh => vh.UserId == userId)
            .OrderByDescending(vh => vh.ViewedAt);

        int? totalCount = search.IncludeTotalCount == true ? await query.CountAsync() : null;

        if (search.Page.HasValue)
        {
            query = query.Skip((search.Page.Value - 1) * (search.PageSize ?? 10));
        }
        if (search.PageSize.HasValue)
        {
            query = query.Take(search.PageSize.Value);
        }

        var items = await query
            .Select(vh => new ViewHistoryResponse
            {
                Id = vh.Id,
                DestinationId = vh.DestinationId,
                DestinationName = vh.Destination.Name,
                ViewedAt = vh.ViewedAt
            })
            .ToListAsync();

        return new PageResult<ViewHistoryResponse> { Items = items, TotalCount = totalCount };
    }
}
