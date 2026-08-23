using TravelBay.Services;
using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;
using Microsoft.AspNetCore.Mvc;
using TravelBay.Model.Requests;

namespace TravelBay.WebAPI.Controllers;

public class UnitOfMeasuresController : BaseCRUDController<UnitOfMeasureResponse, UnitOfMeasureSearch, UnitOfMeasureInsertRequest, UnitOfMeasureUpdateRequest, IUnitOfMeasureService>
{
    public UnitOfMeasuresController(IUnitOfMeasureService unitOfMeasureService) : base(unitOfMeasureService)
    {
    }
}
