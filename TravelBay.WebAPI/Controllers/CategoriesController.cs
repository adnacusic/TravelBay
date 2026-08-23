using TravelBay.Model.Requests;
using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;
using TravelBay.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TravelBay.WebAPI.Controllers;

public class CategoriesController : BaseCRUDController<CategoryResponse, CategorySearchObject, CategoriesInsertRequest, CategoriesUpdateRequest, ICategoryService>
{
    public CategoriesController(ICategoryService categoryService) : base(categoryService)
    {
    }

    [AllowAnonymous]
    public override Task<PageResult<CategoryResponse>> GetAll([FromQuery] CategorySearchObject? search)
    {
        return base.GetAll(search);
    }
}