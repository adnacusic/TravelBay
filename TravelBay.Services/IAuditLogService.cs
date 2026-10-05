using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;

namespace TravelBay.Services;

public interface IAuditLogService
{
    Task<PageResult<AuditLogResponse>> GetAllAsync(AuditLogSearchObject? search = null);

    /// <summary>Internal write hook used by the state machines — there is no public create endpoint.</summary>
    Task LogAsync(string entityName, int entityId, string action, int? performedByUserId, string? details = null);
}
