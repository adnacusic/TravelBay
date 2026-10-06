using TravelBay.Model.Requests;
using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;

namespace TravelBay.Services
{
    public interface INewsService : IBaseCRUDService<NewsResponse, NewsSearchObject, NewsInsertRequest, NewsUpdateRequest>
    {
        /// <summary>Raw bytes of an admin-uploaded news image (external images are served by their own host).</summary>
        Task<(byte[] Content, string ContentType)> GetImageAsync(int id);
    }
}
