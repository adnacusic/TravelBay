using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TravelBay.Services.Database;

namespace TravelBay.WebAPI.Services;

/// <summary>
/// Creates and migrates the database (schema + HasData seed) when the API starts, so a plain
/// <c>docker compose up</c> on a clean machine needs no manual <c>dotnet ef</c> command.
/// SQL Server in Docker can still refuse logins for a while after it reports healthy, so
/// connection failures are retried before the API gives up.
/// </summary>
public static class DatabaseMigrator
{
    private const int MaxAttempts = 10;
    private static readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(5);

    public static async Task MigrateDatabaseAsync(this WebApplication app)
    {
        var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(DatabaseMigrator));

        for (var attempt = 1; ; attempt++)
        {
            try
            {
                await using var scope = app.Services.CreateAsyncScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<TravelBayDbContext>();

                var pending = (await dbContext.Database.GetPendingMigrationsAsync()).ToList();
                if (pending.Count == 0)
                {
                    logger.LogInformation("Database is up to date.");
                    return;
                }

                logger.LogInformation("Applying {Count} migration(s): {Migrations}", pending.Count, string.Join(", ", pending));
                await dbContext.Database.MigrateAsync();
                logger.LogInformation("Database migrated.");
                return;
            }
            catch (SqlException ex) when (attempt < MaxAttempts)
            {
                logger.LogWarning("Database not ready (attempt {Attempt}/{MaxAttempts}): {Message}. Retrying in {Delay}s.",
                    attempt, MaxAttempts, ex.Message, RetryDelay.TotalSeconds);
                await Task.Delay(RetryDelay);
            }
        }
    }
}
