namespace TravelBay.Model.Responses
{
    public class RecommendationResponse
    {
        public DestinationResponse Destination { get; set; } = null!;

        /// <summary>0-100, round(score * 100).</summary>
        public int MatchPercent { get; set; }

        /// <summary>Human-readable "why", generated from the real factor values behind the score.</summary>
        public string Explanation { get; set; } = string.Empty;
    }
}
