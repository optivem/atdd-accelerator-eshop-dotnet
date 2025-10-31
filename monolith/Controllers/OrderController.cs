using Microsoft.AspNetCore.Mvc;
using Optivem.AtddAccelerator.EShop.Monolith.Core.DTOs;
using Optivem.AtddAccelerator.EShop.Monolith.Core.Services;

namespace Optivem.AtddAccelerator.EShop.Monolith.Controllers;

[ApiController]
[Route("api")]
public class OrderController : ControllerBase
{
    private readonly OrderService _orderService;

    public OrderController(OrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost("orders")]
    public async Task<ActionResult<PlaceOrderResponse>> PlaceOrder([FromBody] PlaceOrderRequest request)
    {
        var response = await _orderService.PlaceOrder(request);
        return Ok(response);
    }

    [HttpGet("orders/{orderNumber}")]
    public ActionResult<GetOrderResponse> GetOrder(string orderNumber)
    {
        var response = _orderService.GetOrder(orderNumber);
        return Ok(response);
    }

    [HttpDelete("orders/{orderNumber}")]
    public IActionResult CancelOrder(string orderNumber)
    {
        _orderService.CancelOrder(orderNumber);
        return NoContent();
    }
}
