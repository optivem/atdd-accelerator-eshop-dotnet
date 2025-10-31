using Optivem.AtddAccelerator.EShop.Monolith.Core.Entities;

namespace Optivem.AtddAccelerator.EShop.Monolith.Core.Repositories;

public class OrderRepository
{
    private static readonly Dictionary<string, Order> Orders = new();

    public void AddOrder(Order order)
    {
        if (Orders.ContainsKey(order.OrderNumber))
        {
            throw new ArgumentException($"Order with order number {order.OrderNumber} already exists.");
        }

        Orders[order.OrderNumber] = order;
    }

    public void UpdateOrder(Order order)
    {
        if (!Orders.ContainsKey(order.OrderNumber))
        {
            throw new ArgumentException($"Order with order number {order.OrderNumber} does not exist.");
        }

        Orders[order.OrderNumber] = order;
    }

    public Order? GetOrder(string orderNumber)
    {
        return Orders.GetValueOrDefault(orderNumber);
    }

    public string NextOrderNumber()
    {
        return "ORD-" + Guid.NewGuid();
    }
}
