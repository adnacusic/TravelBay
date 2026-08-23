using TravelBay.Model.Requests;
using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;

namespace TravelBay.Services;

public interface IOrderService : IBaseReadService<OrderResponse, OrderSearchObject>
{
    Task<OrderResponse> CheckoutAsync(CheckoutRequest request);
}
