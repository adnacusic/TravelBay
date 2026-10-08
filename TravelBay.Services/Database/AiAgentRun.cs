using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TravelBay.Model.Enums;

namespace TravelBay.Services.Database
{
    /// <summary>
    /// One start of an AI agent from the desktop. Created by the API (Queued) when it publishes the
    /// RabbitMQ message; the worker then updates status, counters and the log while it processes.
    /// </summary>
    public class AiAgentRun
    {
        [Key]
        public int Id { get; set; }

        public AiAgentType AgentType { get; set; }

        public AiAgentRunStatus Status { get; set; } = AiAgentRunStatus.Queued;

        public int RequestedByUserId { get; set; }

        [ForeignKey("RequestedByUserId")]
        public User RequestedByUser { get; set; } = null!;

        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

        public DateTime? StartedAt { get; set; }

        public DateTime? FinishedAt { get; set; }

        public int? TotalCount { get; set; }

        public int ProcessedCount { get; set; }

        public int SucceededCount { get; set; }

        public int FailedCount { get; set; }

        [Required]
        public string Log { get; set; } = string.Empty;
    }
}
