using System.Globalization;

namespace TravelBay.Services.Recommendation;

/// <summary>Pure scoring + explanation logic (no I/O). Implements recommender-dokumentacija.md §3–§5.</summary>
public static class RecommendationScorer
{
    public static ScoredCandidate Score(CandidateRow candidate, UserSignals user, GlobalStats stats)
    {
        var isColdStart = !user.HasPreferences && !user.HasHistory;

        var pref = user.PreferredCategoryIds.Contains(candidate.CategoryId) ? 1.0 : 0.0;
        var behavior = Behavior(user, candidate.CategoryId);
        var category = isColdStart ? 0.0 : CategoryScore(user, pref, behavior);
        var rating = RatingScore(candidate, stats);
        var popularity = PopularityScore(candidate, stats);

        // Cold start has no category signal: its weight is shared proportionally between the other two.
        var otherWeights = RecommenderSettings.RatingWeight + RecommenderSettings.PopularityWeight;
        var wCategory = isColdStart ? 0.0 : RecommenderSettings.CategoryWeight;
        var wRating = isColdStart ? RecommenderSettings.RatingWeight / otherWeights : RecommenderSettings.RatingWeight;
        var wPopularity = isColdStart ? RecommenderSettings.PopularityWeight / otherWeights : RecommenderSettings.PopularityWeight;

        var score = wCategory * category + wRating * rating + wPopularity * popularity;
        var matchPercent = (int)Math.Clamp(Math.Round(score * 100, MidpointRounding.AwayFromZero), 0, 100);

        var explanation = BuildExplanation(candidate, pref, behavior, user, rating * wRating, popularity * wPopularity, popularity);

        return new ScoredCandidate(candidate, score, matchPercent, explanation);
    }

    private static double Behavior(UserSignals user, int categoryId)
    {
        if (!user.HasHistory)
        {
            return 0.0;
        }

        return user.ViewsByCategory.TryGetValue(categoryId, out var views) ? (double)views / user.MaxViews : 0.0;
    }

    private static double CategoryScore(UserSignals user, double pref, double behavior)
    {
        if (user.HasPreferences && user.HasHistory)
        {
            return RecommenderSettings.PreferenceShare * pref + RecommenderSettings.BehaviorShare * behavior;
        }

        return user.HasPreferences ? pref : behavior;
    }

    private static double RatingScore(CandidateRow candidate, GlobalStats stats)
    {
        var n = candidate.ApprovedReviewCount;
        var average = n > 0 ? (double)candidate.ApprovedRatingSum / n : 0.0;
        var m = RecommenderSettings.RatingPriorWeight;
        var smoothed = (n * average + m * stats.GlobalAverageRating) / (n + m);

        var normalized = (smoothed - RecommenderSettings.MinRating) / (RecommenderSettings.MaxRating - RecommenderSettings.MinRating);
        return Math.Clamp(normalized, 0.0, 1.0);
    }

    private static double PopularityScore(CandidateRow candidate, GlobalStats stats)
    {
        if (stats.RawMax == stats.RawMin)
        {
            return 0.0;
        }

        var raw = candidate.UniqueVisitors + candidate.ApprovedReviewCount;
        var low = Math.Log(1 + stats.RawMin);
        var high = Math.Log(1 + stats.RawMax);

        return Math.Clamp((Math.Log(1 + raw) - low) / (high - low), 0.0, 1.0);
    }

    private static string BuildExplanation(
        CandidateRow candidate, double pref, double behavior, UserSignals user,
        double ratingContribution, double popularityContribution, double popularity)
    {
        var categoryPhrase = CategoryPhrase(candidate.CategoryName, pref, behavior, user);

        var attributes = new List<(double Contribution, string Text)>();
        if (candidate.ApprovedReviewCount >= 1)
        {
            var average = (double)candidate.ApprovedRatingSum / candidate.ApprovedReviewCount;
            if (average >= RecommenderSettings.HighRatingThreshold)
            {
                attributes.Add((ratingContribution, $"visoko ocijenjena ({average.ToString("0.0", CultureInfo.InvariantCulture)}★)"));
            }
        }
        if (popularity >= RecommenderSettings.HighPopularityThreshold)
        {
            attributes.Add((popularityContribution, "popularna među putnicima"));
        }

        var attributeText = string.Join(" i ", attributes.OrderByDescending(a => a.Contribution).Select(a => a.Text));

        return (categoryPhrase, attributeText) switch
        {
            (null, "") => "Preporučeno na osnovu ocjena i popularnosti.",
            (not null, "") => $"Preporučeno jer {categoryPhrase}.",
            (null, _) => $"Preporučeno jer je destinacija {attributeText}.",
            _ => $"Preporučeno jer {categoryPhrase}, a destinacija je {attributeText}."
        };
    }

    private static string? CategoryPhrase(string categoryName, double pref, double behavior, UserSignals user)
    {
        var likes = pref >= 1.0;
        var oftenViews = user.HasHistory && behavior >= RecommenderSettings.FrequentViewThreshold;

        return (likes, oftenViews) switch
        {
            (true, true) => $"voliš i često pregledaš kategoriju „{categoryName}“",
            (true, false) => $"voliš kategoriju „{categoryName}“",
            (false, true) => $"često pregledaš kategoriju „{categoryName}“",
            _ => null
        };
    }
}
