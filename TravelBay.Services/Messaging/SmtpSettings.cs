namespace TravelBay.Services.Messaging;

/// <summary>
/// The worker sends the emails; the API only needs to know whether SMTP is set up in .env, so that
/// "Zaboravljena lozinka" fails with a clear message instead of a code that never arrives.
/// </summary>
public class SmtpSettings
{
    public string? Host { get; init; }
    public string? From { get; init; }

    public bool IsConfigured => !string.IsNullOrWhiteSpace(Host) && !string.IsNullOrWhiteSpace(From);
}
