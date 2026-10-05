namespace TravelBay.Services.Recommendation;

/// <summary>Every number used by the recommender, in one place. Values mirror recommender-dokumentacija.md §3.5.</summary>
public static class RecommenderSettings
{
    public const double CategoryWeight = 0.5;
    public const double RatingWeight = 0.3;
    public const double PopularityWeight = 0.2;

    public const double PreferenceShare = 0.7;
    public const double BehaviorShare = 0.3;

    public const double RatingPriorWeight = 3;
    public const double DefaultGlobalAverageRating = 3.0;
    public const double MinRating = 1;
    public const double MaxRating = 5;

    public const double HighRatingThreshold = 4.0;
    public const double HighPopularityThreshold = 0.6;
    public const double FrequentViewThreshold = 0.5;

    public const int MaxCandidates = 200;
    public const int DefaultPageSize = 10;
    public const int MaxPageSize = 50;

    public const int StatsCacheMinutes = 5;
}
