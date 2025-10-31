using Optivem.AtddAccelerator.EShop.Monolith.Core.Entities;

namespace Optivem.AtddAccelerator.EShop.Monolith.Core.DTOs;

public class GetOrderResponse
{
    public string OrderNumber { get; set; } = string.Empty;
    public long ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public OrderStatus Status { get; set; }
}
