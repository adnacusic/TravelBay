using TravelBay.Services;
using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;
using Microsoft.AspNetCore.Mvc;
using TravelBay.Model.Requests;

namespace TravelBay.WebAPI.Controllers;

public class DestinationsController : BaseCRUDController<DestinationResponse, DestinationSearchObject, DestinationInsertRequest, DestinationUpdateRequest, IDestinationService>
{
    public DestinationsController(IDestinationService destinationService) : base(destinationService)
    {
    }
}
