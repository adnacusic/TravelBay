namespace TravelBay.Services.Messaging;

/// <summary>Puts a job on a RabbitMQ queue consumed by the Python worker (AI agents, emails).</summary>
public interface IMessageQueuePublisher
{
    /// <param name="queue">AiAgentMessages.QueueName or EmailMessages.QueueName.</param>
    /// <param name="message">Serialized as JSON; the worker reads it by the same property names.</param>
    Task PublishAsync(string queue, object message, CancellationToken cancellationToken = default);
}
