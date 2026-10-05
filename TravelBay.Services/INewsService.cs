using TravelBay.Model.Requests;
using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;

namespace TravelBay.Services
{
    public interface INewsService : IBaseCRUDService<NewsResponse, NewsSearchObject, NewsInsertRequest, NewsUpdateRequest>
    {
    }
}
