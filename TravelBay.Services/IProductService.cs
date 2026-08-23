using TravelBay.Model.Requests;
using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;

namespace TravelBay.Services
{
    public interface IProductService : IBaseCRUDService<ProductResponse, ProductSearchObject, ProductInsertRequest, ProductUpdateRequest>
    {
        Task<ProductResponse> ActivateAsync(int id);
        Task<ProductResponse> DeactivateAsync(int id);

        Task<List<string>> GetAllowedActionsAsync(int id);
    }
}