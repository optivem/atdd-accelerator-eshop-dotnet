using Optivem.AtddAccelerator.EShop.Monolith.Core.DTOs;
using Optivem.AtddAccelerator.EShop.Monolith.Core.Entities;
using Optivem.AtddAccelerator.EShop.Monolith.Core.Exceptions;
using Optivem.AtddAccelerator.EShop.Monolith.Core.Repositories;
using Optivem.AtddAccelerator.EShop.Monolith.Core.Services.External;

namespace Optivem.AtddAccelerator.EShop.Monolith.Core.Services;

public class OrderService
{
    public static readonly (int Month, int Day) December31 = (12, 31);
    private static readonly TimeSpan CancellationBlockStart = new(22, 0, 0);
    private static readonly TimeSpan CancellationBlockEnd = new(23, 0, 0);

    private readonly OrderRepository _orderRepository;
    private readonly ErpGateway _erpGateway;

    public OrderService(OrderRepository orderRepository, ErpGateway erpGateway)
    {
        _orderRepository = orderRepository;
        _erpGateway = erpGateway;
    }

    public async Task<PlaceOrderResponse> PlaceOrder(PlaceOrderRequest request)
    {
        var productId = request.ProductId;
        var quantity = request.Quantity;

        if (productId <= 0)
        {
            throw new ValidationException($"Product ID must be greater than 0, received: {productId}");
        }
        if (quantity <= 0)
        {
            throw new ValidationException($"Quantity must be greater than 0, received: {quantity}");
        }

        var orderNumber = _orderRepository.NextOrderNumber();
        var unitPrice = await _erpGateway.GetUnitPrice(productId);
        var totalPrice = unitPrice * quantity;
        var order = new Order(orderNumber, productId, quantity, unitPrice, totalPrice, OrderStatus.Placed);

        _orderRepository.AddOrder(order);

        var response = new PlaceOrderResponse
        {
            OrderNumber = orderNumber,
            TotalPrice = totalPrice
        };
        return response;
    }

    public GetOrderResponse GetOrder(string orderNumber)
    {
        var order = _orderRepository.GetOrder(orderNumber);
        if (order == null)
        {
            throw new KeyNotFoundException($"Order with order number {orderNumber} not found.");
        }

        var response = new GetOrderResponse
        {
            OrderNumber = orderNumber,
            ProductId = order.ProductId,
            Quantity = order.Quantity,
            UnitPrice = order.UnitPrice,
            TotalPrice = order.TotalPrice,
            Status = order.Status
        };

        return response;
    }

    public void CancelOrder(string orderNumber)
    {
        var now = DateTime.Now;
        var currentDate = (now.Month, now.Day);
        var currentTime = now.TimeOfDay;

        if (currentDate == December31 &&
            currentTime > CancellationBlockStart &&
            currentTime < CancellationBlockEnd)
        {
            throw new ValidationException("Order cancellation is not allowed on December 31st between 22:00 and 23:00");
        }

        var order = _orderRepository.GetOrder(orderNumber);
        if (order == null)
        {
            throw new KeyNotFoundException($"Order with order number {orderNumber} not found.");
        }

        order.Status = OrderStatus.Cancelled;
        _orderRepository.UpdateOrder(order);
    }
}
