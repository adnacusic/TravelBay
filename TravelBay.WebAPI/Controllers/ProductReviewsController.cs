using TravelBay.Model.Requests;
using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;
using TravelBay.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TravelBay.WebAPI.Controllers;

[Authorize]
public class ProductReviewsController
    : BaseCRUDController<ProductReviewResponse, ProductReviewSearchObject, ProductReviewInsertRequest, ProductReviewUpdateRequest, IProductReviewService>
{
    public ProductReviewsController(IProductReviewService productReviewService)
        : base(productReviewService)
    {
    }
}
