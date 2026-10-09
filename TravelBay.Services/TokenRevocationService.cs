using Microsoft.EntityFrameworkCore;
using TravelBay.Services.Database;

namespace TravelBay.Services;

public class TokenRevocationService : ITokenRevocationService
{
    private readonly TravelBayDbContext _dbContext;

    public TokenRevocationService(TravelBayDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task RevokeAsync(int userId, string jti, DateTime expiresAt)
    {
        var now = DateTime.UtcNow;
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();

        await _dbContext.RefreshTokens.Where(rt => rt.UserId == userId).ExecuteDeleteAsync();
        // Expired tokens are rejected anyway, so their blacklist entries are no longer needed.
        await _dbContext.RevokedAccessTokens.Where(t => t.ExpiresAt < now).ExecuteDeleteAsync();

        var alreadyRevoked = await _dbContext.RevokedAccessTokens.AnyAsync(t => t.Jti == jti);
        if (!alreadyRevoked)
        {
            _dbContext.RevokedAccessTokens.Add(new RevokedAccessToken
            {
                Jti = jti,
                UserId = userId,
                RevokedAt = now,
                ExpiresAt = expiresAt
            });
            await _dbContext.SaveChangesAsync();
        }

        await transaction.CommitAsync();
    }

    public Task<bool> IsRevokedAsync(string jti) =>
        _dbContext.RevokedAccessTokens.AsNoTracking().AnyAsync(t => t.Jti == jti);
}
