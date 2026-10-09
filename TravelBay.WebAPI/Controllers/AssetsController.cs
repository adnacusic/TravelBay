using TravelBay.Model.Constants;
using TravelBay.Services;
using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelBay.Model.Requests;

namespace TravelBay.WebAPI.Controllers;

/// <summary>
/// Raw asset store, for administration only: users never touch assets directly — profile pictures go
/// through Users/Me/ProfileImage and destination/news images through their own endpoints.
/// </summary>
[Authorize(Roles = RoleNames.Admin)]
public class AssetsController : BaseCRUDController<AssetResponse, AssetSearch, AssetInsertRequest, AssetUpdateRequest, IAssetService>
{
    public AssetsController(IAssetService assetService) : base(assetService)
    {
    }
}
