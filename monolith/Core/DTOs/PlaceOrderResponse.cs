namespace Optivem.AtddAccelerator.EShop.Monolith.Core.DTOs;

public class PlaceOrderResponse
{
    public string OrderNumber { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
}
