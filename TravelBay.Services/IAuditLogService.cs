using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;

namespace TravelBay.Services;

public interface IAuditLogService
{
    Task<PageResult<AuditLogResponse>> GetAllAsync(AuditLogSearchObject? search = null);

    /// <summary>
    /// Internal write hook (no public create endpoint). Only adds the entry to the current unit of work:
    /// the caller saves it in the same SaveChanges as the change it describes, so both are stored or neither.
    /// </summary>
    void Log(string entityName, int entityId, string action, int? performedByUserId, string? details = null);
}
