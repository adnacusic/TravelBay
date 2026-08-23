using TravelBay.Model.Requests;
using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;

namespace TravelBay.Services
{
    public interface ICategoryService : IBaseCRUDService<CategoryResponse, CategorySearchObject, CategoriesInsertRequest, CategoriesUpdateRequest>
    {

    }
}