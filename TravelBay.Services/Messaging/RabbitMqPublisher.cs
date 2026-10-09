using System.Text.Json;
using RabbitMQ.Client;

namespace TravelBay.Services.Messaging;

/// <summary>
/// Publishes JSON to a durable queue as a persistent message, so a job survives a broker restart and
/// waits in the queue while the worker is down. Property names are camelCase (e.g. "runId").
/// </summary>
public class RabbitMqPublisher : IMessageQueuePublisher
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly RabbitMqConnectionProvider _connectionProvider;

    public RabbitMqPublisher(RabbitMqConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async Task PublishAsync(string queue, object message, CancellationToken cancellationToken = default)
    {
        var connection = await _connectionProvider.GetConnectionAsync(cancellationToken);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(
            queue: queue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken);

        var body = JsonSerializer.SerializeToUtf8Bytes(message, message.GetType(), JsonOptions);
        var properties = new BasicProperties
        {
            Persistent = true,
            ContentType = "application/json"
        };

        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: queue,
            mandatory: false,
            basicProperties: properties,
            body: body,
            cancellationToken: cancellationToken);
    }
}
