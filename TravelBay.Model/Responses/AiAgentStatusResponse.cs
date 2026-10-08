namespace TravelBay.Model.Responses
{
    /// <summary>What is still waiting for each agent, and the latest run of each.</summary>
    public class AiAgentStatusResponse
    {
        public int DestinationsWithoutKeywords { get; set; }
        public int DestinationsWithoutImages { get; set; }
        public AiAgentRunResponse? LastKeywordsRun { get; set; }
        public AiAgentRunResponse? LastImagesRun { get; set; }
    }
}
