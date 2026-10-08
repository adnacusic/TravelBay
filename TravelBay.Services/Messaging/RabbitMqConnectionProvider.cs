using RabbitMQ.Client;

namespace TravelBay.Services.Messaging;

/// <summary>
/// One shared RabbitMQ connection for the whole API (registered as a singleton): connections are
/// expensive, channels are cheap, so publishers open a channel per message on this connection.
/// The connection is opened on first use and opened again if the broker dropped it.
/// </summary>
public sealed class RabbitMqConnectionProvider : IAsyncDisposable
{
    private readonly ConnectionFactory _factory;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private IConnection? _connection;

    public RabbitMqConnectionProvider(RabbitMqSettings settings)
    {
        _factory = new ConnectionFactory
        {
            HostName = settings.HostName,
            Port = settings.Port,
            UserName = settings.UserName,
            Password = settings.Password,
            ClientProvidedName = "travelbay-api"
        };
    }

    public async Task<IConnection> GetConnectionAsync(CancellationToken cancellationToken = default)
    {
        if (_connection is { IsOpen: true })
        {
            return _connection;
        }

        await _lock.WaitAsync(cancellationToken);
        try
        {
            if (_connection is { IsOpen: true })
            {
                return _connection;
            }

            if (_connection != null)
            {
                await _connection.DisposeAsync();
            }

            _connection = await _factory.CreateConnectionAsync(cancellationToken);
            return _connection;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection != null)
        {
            await _connection.DisposeAsync();
        }
        _lock.Dispose();
    }
}
