using TravelBay.Model.Requests;
using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;

namespace TravelBay.Services;

/// <summary>Current-user scoped.</summary>
public interface IViewHistoryService
{
    Task<ViewHistoryResponse> RecordAsync(ViewHistoryInsertRequest request);
    Task<PageResult<ViewHistoryResponse>> GetAllAsync(ViewHistorySearchObject? search = null);
}
