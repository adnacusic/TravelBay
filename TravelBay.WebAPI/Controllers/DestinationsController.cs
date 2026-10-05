using TravelBay.Model.Constants;
using TravelBay.Services;
using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelBay.Model.Requests;

namespace TravelBay.WebAPI.Controllers;

[Authorize(Roles = RoleNames.Admin)]
public class DestinationsController : BaseCRUDController<DestinationResponse, DestinationSearchObject, DestinationInsertRequest, DestinationUpdateRequest, IDestinationService>
{
    public DestinationsController(IDestinationService destinationService) : base(destinationService)
    {
    }

    [AllowAnonymous]
    public override Task<PageResult<DestinationResponse>> GetAll([FromQuery] DestinationSearchObject? search) => base.GetAll(search);

    [AllowAnonymous]
    public override Task<ActionResult<DestinationResponse>> GetById(int id) => base.GetById(id);
}
