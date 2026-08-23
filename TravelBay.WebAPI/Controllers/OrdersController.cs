using TravelBay.Model.Requests;
using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;
using TravelBay.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TravelBay.WebAPI.Controllers;

[Authorize]
public class OrdersController : BaseReadController<OrderResponse, OrderSearchObject, IOrderService>
{
    public OrdersController(IOrderService orderService)
        : base(orderService)
    {
    }

    [HttpPost("Checkout")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OrderResponse>> Checkout([FromBody] CheckoutRequest request)
    {
        var result = await _service.CheckoutAsync(request);
        return Ok(result);
    }
}
