namespace TravelBay.Services.Recommendation;

/// <summary>Lightweight per-destination aggregates computed in SQL (no entities loaded for scoring).</summary>
public record CandidateRow(
    int Id,
    int CategoryId,
    string CategoryName,
    int ApprovedReviewCount,
    int ApprovedRatingSum,
    int UniqueVisitors);

/// <summary>Values that are identical for every user, cached for <see cref="RecommenderSettings.StatsCacheMinutes"/>.</summary>
public record GlobalStats(int RawMin, int RawMax, double GlobalAverageRating);

/// <summary>What we know about the current user: explicit preferences and per-category view counts.</summary>
public class UserSignals
{
    public HashSet<int> PreferredCategoryIds { get; init; } = new();
    public Dictionary<int, int> ViewsByCategory { get; init; } = new();

    public bool HasPreferences => PreferredCategoryIds.Count > 0;
    public int MaxViews => ViewsByCategory.Count > 0 ? ViewsByCategory.Values.Max() : 0;
    public bool HasHistory => MaxViews > 0;
}

public record ScoredCandidate(CandidateRow Candidate, double Score, int MatchPercent, string Explanation);
