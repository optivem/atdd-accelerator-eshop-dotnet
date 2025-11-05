namespace Optivem.AtddAccelerator.EShop.SystemTest.Core.Clients.Api.Dtos;

public class GetOrderResponse
{
    public string OrderNumber { get; set; } = string.Empty;
    public long ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = string.Empty;
}
