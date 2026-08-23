using TravelBay.Services;
using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;
using Microsoft.AspNetCore.Mvc;
using TravelBay.Model.Requests;

namespace TravelBay.WebAPI.Controllers;

public class AssetsController : BaseCRUDController<AssetResponse, AssetSearch, AssetInsertRequest, AssetUpdateRequest, IAssetService>
{
    public AssetsController(IAssetService assetService) : base(assetService)
    {
    }
}
