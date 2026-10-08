using TravelBay.Model.Enums;

namespace TravelBay.Model.Responses
{
    public class AiAgentRunResponse
    {
        public int Id { get; set; }
        public AiAgentType AgentType { get; set; }
        public AiAgentRunStatus Status { get; set; }
        public string RequestedByDisplayName { get; set; } = string.Empty;
        public DateTime RequestedAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }

        /// <summary>Destinations the worker found to process; null until it starts.</summary>
        public int? TotalCount { get; set; }
        public int ProcessedCount { get; set; }
        public int SucceededCount { get; set; }
        public int FailedCount { get; set; }

        /// <summary>One line per step, written by the worker as it goes.</summary>
        public string Log { get; set; } = string.Empty;
    }
}
