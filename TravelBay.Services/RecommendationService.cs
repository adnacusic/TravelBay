using TravelBay.Model.Enums;
using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;
using TravelBay.Services.Database;
using TravelBay.Services.Recommendation;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace TravelBay.Services;

/// <summary>Content-based + popularity recommender; behaviour is specified in recommender-dokumentacija.md.</summary>
public class RecommendationService : IRecommendationService
{
    private const string StatsCacheKey = "recommender:global-stats";

    private readonly TravelBayDbContext _dbContext;
    private readonly IAuthenticatedUserAccessor _userAccessor;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;

    public RecommendationService(TravelBayDbContext dbContext, IAuthenticatedUserAccessor userAccessor, IMapper mapper, IMemoryCache cache)
    {
        _dbContext = dbContext;
        _userAccessor = userAccessor;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<PageResult<RecommendationResponse>> GetRecommendationsAsync(RecommendationSearchObject? search = null)
    {
        var userId = _userAccessor.GetUserId() ?? throw new InvalidOperationException("User id claim is missing.");

        var page = Math.Max(1, search?.Page ?? 1);
        var pageSize = Math.Clamp(search?.PageSize ?? RecommenderSettings.DefaultPageSize, 1, RecommenderSettings.MaxPageSize);

        var user = await LoadUserSignalsAsync(userId);
        var stats = await GetGlobalStatsAsync();
        var candidates = await LoadCandidatesAsync(userId);

        var ranked = candidates
            .Select(c => RecommendationScorer.Score(c, user, stats))
            .OrderByDescending(s => s.Score)
            .ThenBy(s => s.Candidate.Id)
            .ToList();

        var pageItems = ranked.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        var items = await BuildResponsesAsync(pageItems);

        return new PageResult<RecommendationResponse> { Items = items, TotalCount = ranked.Count };
    }

    private async Task<UserSignals> LoadUserSignalsAsync(int userId)
    {
        var preferred = await _dbContext.UserPreferences
            .AsNoTracking()
            .Where(up => up.UserId == userId)
            .Select(up => up.CategoryId)
            .ToListAsync();

        var viewsByCategory = await _dbContext.ViewHistories
            .AsNoTracking()
            .Where(vh => vh.UserId == userId)
            .GroupBy(vh => vh.Destination.CategoryId)
            .Select(g => new { CategoryId = g.Key, Views = g.Count() })
            .ToListAsync();

        return new UserSignals
        {
            PreferredCategoryIds = preferred.ToHashSet(),
            ViewsByCategory = viewsByCategory.ToDictionary(v => v.CategoryId, v => v.Views)
        };
    }

    private async Task<GlobalStats> GetGlobalStatsAsync()
    {
        var stats = await _cache.GetOrCreateAsync(StatsCacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(RecommenderSettings.StatsCacheMinutes);

            var raw = _dbContext.Destinations
                .AsNoTracking()
                .Select(d => d.ViewHistoryEntries.Select(v => v.UserId).Distinct().Count()
                             + d.Reviews.Count(r => r.Status == ReviewStatus.Approved));

            var min = await raw.MinAsync();
            var max = await raw.MaxAsync();

            var average = await _dbContext.Reviews
                .AsNoTracking()
                .Where(r => r.Status == ReviewStatus.Approved)
                .AverageAsync(r => (double?)r.Rating);

            return new GlobalStats(min, max, average ?? RecommenderSettings.DefaultGlobalAverageRating);
        });

        return stats!;
    }

    private async Task<List<CandidateRow>> LoadCandidatesAsync(int userId)
    {
        // One SQL query: aggregates are correlated sub-selects, saved destinations are a NOT EXISTS.
        var rows = await _dbContext.Destinations
            .AsNoTracking()
            .Where(d => d.Category.IsActive
                        && !_dbContext.SavedDestinations.Any(sd => sd.UserId == userId && sd.DestinationId == d.Id))
            .Select(d => new
            {
                d.Id,
                d.CategoryId,
                CategoryName = d.Category.Name,
                ApprovedReviewCount = d.Reviews.Count(r => r.Status == ReviewStatus.Approved),
                ApprovedRatingSum = d.Reviews.Where(r => r.Status == ReviewStatus.Approved).Sum(r => (int?)r.Rating) ?? 0,
                UniqueVisitors = d.ViewHistoryEntries.Select(v => v.UserId).Distinct().Count()
            })
            .OrderByDescending(c => c.UniqueVisitors + c.ApprovedReviewCount)
            .ThenBy(c => c.Id)
            .Take(RecommenderSettings.MaxCandidates)
            .ToListAsync();

        return rows
            .Select(r => new CandidateRow(r.Id, r.CategoryId, r.CategoryName, r.ApprovedReviewCount, r.ApprovedRatingSum, r.UniqueVisitors))
            .ToList();
    }

    private async Task<List<RecommendationResponse>> BuildResponsesAsync(List<ScoredCandidate> pageItems)
    {
        if (pageItems.Count == 0)
        {
            return new List<RecommendationResponse>();
        }

        var ids = pageItems.Select(p => p.Candidate.Id).ToList();
        var destinations = await _dbContext.Destinations
            .AsNoTracking()
            .Include(d => d.Category)
            .Include(d => d.Images)
            .Where(d => ids.Contains(d.Id))
            .ToDictionaryAsync(d => d.Id);

        return pageItems.Select(p =>
        {
            var destination = _mapper.Map<DestinationResponse>(destinations[p.Candidate.Id]);
            destination.ReviewCount = p.Candidate.ApprovedReviewCount;
            destination.AverageRating = p.Candidate.ApprovedReviewCount > 0
                ? (double)p.Candidate.ApprovedRatingSum / p.Candidate.ApprovedReviewCount
                : null;

            return new RecommendationResponse
            {
                Destination = destination,
                MatchPercent = p.MatchPercent,
                Explanation = p.Explanation
            };
        }).ToList();
    }
}
