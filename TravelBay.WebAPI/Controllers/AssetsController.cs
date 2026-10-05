using TravelBay.Model.Constants;
using TravelBay.Services;
using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelBay.Model.Requests;

namespace TravelBay.WebAPI.Controllers;

[Authorize]
public class AssetsController : BaseCRUDController<AssetResponse, AssetSearch, AssetInsertRequest, AssetUpdateRequest, IAssetService>
{
    public AssetsController(IAssetService assetService) : base(assetService)
    {
    }

    [Authorize(Roles = RoleNames.Admin)]
    public override Task<PageResult<AssetResponse>> GetAll([FromQuery] AssetSearch? search) => base.GetAll(search);

    [Authorize(Roles = RoleNames.Admin)]
    public override Task<IActionResult> Delete(int id) => base.Delete(id);
}
