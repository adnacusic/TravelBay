using System.Text.Json;
using RabbitMQ.Client;
using TravelBay.Model.Constants;

namespace TravelBay.Services.Messaging;

/// <summary>
/// Publishes {"command": ..., "runId": ...} to the durable queue as a persistent message, so a job
/// survives a broker restart and waits in the queue while the worker is down.
/// </summary>
public class AiAgentQueuePublisher : IAiAgentQueuePublisher
{
    private readonly RabbitMqConnectionProvider _connectionProvider;

    public AiAgentQueuePublisher(RabbitMqConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async Task PublishAsync(string command, int runId, CancellationToken cancellationToken = default)
    {
        var connection = await _connectionProvider.GetConnectionAsync(cancellationToken);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(
            queue: AiAgentMessages.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken);

        var body = JsonSerializer.SerializeToUtf8Bytes(new { command, runId });
        var properties = new BasicProperties
        {
            Persistent = true,
            ContentType = "application/json"
        };

        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: AiAgentMessages.QueueName,
            mandatory: false,
            basicProperties: properties,
            body: body,
            cancellationToken: cancellationToken);
    }
}
