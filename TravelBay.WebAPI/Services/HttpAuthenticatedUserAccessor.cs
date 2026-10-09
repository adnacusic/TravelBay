using System.Security.Claims;
using TravelBay.Model.Constants;
using TravelBay.Services;

namespace TravelBay.WebAPI.Services;

public class HttpAuthenticatedUserAccessor : IAuthenticatedUserAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpAuthenticatedUserAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int? GetUserId()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        var id = user.FindFirstValue(ClaimNames.Id) ?? user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(id) || !int.TryParse(id, out var userId))
        {
            return null;
        }

        return userId;
    }

    public bool IsInRole(string role)
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated != true)
        {
            return false;
        }

        var userRole = user.FindFirstValue(ClaimNames.Role) ?? user.FindFirstValue("role");
        return userRole != null && userRole == role;
    }

    public string? GetTokenId() => _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimNames.TokenId);

    public DateTime? GetTokenExpiresAt()
    {
        var exp = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimNames.ExpiresAt);
        return long.TryParse(exp, out var seconds) ? DateTimeOffset.FromUnixTimeSeconds(seconds).UtcDateTime : null;
    }
}
