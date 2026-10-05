using TravelBay.Model.Constants;
using TravelBay.Model.Requests;
using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;
using TravelBay.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TravelBay.WebAPI.Controllers;

[Authorize(Roles = RoleNames.Admin)]
public class CitiesController : BaseCRUDController<CityResponse, CitySearchObject, CityInsertRequest, CityUpdateRequest, ICityService>
{
    public CitiesController(ICityService cityService) : base(cityService)
    {
    }

    [AllowAnonymous]
    public override Task<PageResult<CityResponse>> GetAll([FromQuery] CitySearchObject? search) => base.GetAll(search);

    [AllowAnonymous]
    public override Task<ActionResult<CityResponse>> GetById(int id) => base.GetById(id);
}
