using TravelBay.Model.Responses;
using TravelBay.Services.Database;
using Microsoft.EntityFrameworkCore;

namespace TravelBay.Services;

public class DashboardService : IDashboardService
{
    private const int RecentDestinationCount = 10;

    private readonly TravelBayDbContext _dbContext;

    public DashboardService(TravelBayDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DashboardStatsResponse> GetStatsAsync()
    {
        // One aggregate query over Destinations (soft-deleted rows are excluded by the global filter).
        var counts = await _dbContext.Destinations
            .AsNoTracking()
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Total = g.Count(),
                WithImages = g.Count(d => d.Images.Any()),
                WithKeywords = g.Count(d => d.Keywords != null && d.Keywords != ""),
                Processed = g.Count(d => d.Images.Any() && d.Keywords != null && d.Keywords != "")
            })
            .FirstOrDefaultAsync();

        var totalUsers = await _dbContext.Users.AsNoTracking().CountAsync();

        var recent = await _dbContext.Destinations
            .AsNoTracking()
            .OrderByDescending(d => d.CreatedAt)
            .ThenByDescending(d => d.Id)
            .Take(RecentDestinationCount)
            .Select(d => new RecentDestinationResponse
            {
                Id = d.Id,
                Name = d.Name,
                CategoryName = d.Category.Name,
                CityName = d.City.Name,
                CreatedAt = d.CreatedAt,
                HasKeywords = d.Keywords != null && d.Keywords != "",
                HasImages = d.Images.Any()
            })
            .ToListAsync();

        var total = counts?.Total ?? 0;
        var withImages = counts?.WithImages ?? 0;
        var withKeywords = counts?.WithKeywords ?? 0;
        var processed = counts?.Processed ?? 0;

        return new DashboardStatsResponse
        {
            TotalDestinations = total,
            TotalUsers = totalUsers,
            DestinationsWithImages = withImages,
            DestinationsWithoutImages = total - withImages,
            DestinationsWithKeywords = withKeywords,
            DestinationsWithoutKeywords = total - withKeywords,
            ProcessedDestinations = processed,
            ProcessedPercent = total == 0 ? 0 : Math.Round(processed * 100.0 / total, 1),
            RecentDestinations = recent
        };
    }
}
