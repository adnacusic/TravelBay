namespace TravelBay.Services.Messaging;

/// <summary>Puts an AI agent job on the RabbitMQ queue the Python worker consumes.</summary>
public interface IAiAgentQueuePublisher
{
    /// <param name="command">AiAgentMessages.GenerateKeywords or AiAgentMessages.FindImages.</param>
    /// <param name="runId">The AiAgentRun row the worker reports its progress into.</param>
    Task PublishAsync(string command, int runId, CancellationToken cancellationToken = default);
}
