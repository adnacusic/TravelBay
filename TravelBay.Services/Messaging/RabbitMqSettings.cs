namespace TravelBay.Services.Messaging;

/// <summary>RabbitMQ connection data; Program.cs fills it from .env (never hardcoded).</summary>
public class RabbitMqSettings
{
    public string HostName { get; init; } = string.Empty;
    public int Port { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
