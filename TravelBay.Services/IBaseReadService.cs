using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TravelBay.Services
{
    public interface IBaseReadService<TResponse, TSearch>
        where TSearch : BaseSearchObject
    {
        Task<TResponse> GetByIdAsync(int id);
        Task<PageResult<TResponse>> GetAllAsync(TSearch? search = null);
    }
}
