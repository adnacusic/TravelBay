using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;
using TravelBay.Services.Database;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace TravelBay.Services;

public class AuditLogService : IAuditLogService
{
    private readonly TravelBayDbContext _dbContext;
    private readonly IMapper _mapper;

    public AuditLogService(TravelBayDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<PageResult<AuditLogResponse>> GetAllAsync(AuditLogSearchObject? search = null)
    {
        search ??= new AuditLogSearchObject();

        IQueryable<AuditLog> query = _dbContext.AuditLogs
            .AsNoTracking()
            .Include(a => a.PerformedByUser);

        if (!string.IsNullOrWhiteSpace(search.EntityName))
        {
            query = query.Where(a => a.EntityName == search.EntityName);
        }
        if (search.EntityId.HasValue)
        {
            query = query.Where(a => a.EntityId == search.EntityId.Value);
        }

        query = query.OrderByDescending(a => a.PerformedAt);

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
            .Select(a => new AuditLogResponse
            {
                Id = a.Id,
                EntityName = a.EntityName,
                EntityId = a.EntityId,
                Action = a.Action,
                PerformedByUserId = a.PerformedByUserId,
                PerformedByUserName = a.PerformedByUser != null ? (a.PerformedByUser.FirstName + " " + a.PerformedByUser.LastName) : null,
                PerformedAt = a.PerformedAt,
                Details = a.Details
            })
            .ToListAsync();

        return new PageResult<AuditLogResponse> { Items = items, TotalCount = totalCount };
    }

    public async Task LogAsync(string entityName, int entityId, string action, int? performedByUserId, string? details = null)
    {
        _dbContext.AuditLogs.Add(new AuditLog
        {
            EntityName = entityName,
            EntityId = entityId,
            Action = action,
            PerformedByUserId = performedByUserId,
            PerformedAt = DateTime.UtcNow,
            Details = details
        });
        await _dbContext.SaveChangesAsync();
    }
}
