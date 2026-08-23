using TravelBay.Services;
using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;
using Microsoft.AspNetCore.Mvc;
using TravelBay.Model.Requests;

namespace TravelBay.WebAPI.Controllers;

public class ProductTypesController : BaseCRUDController<ProductTypeResponse, ProductTypeSearch, ProductTypeInsertRequest, ProductTypeUpdateRequest, IProductTypeService>
{
    public ProductTypesController(IProductTypeService productTypeService) : base(productTypeService)
    {
    }
}
