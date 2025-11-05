namespace Optivem.AtddAccelerator.EShop.SystemTest.Clients;

public class PlaceOrderRequest
{
    public long ProductId { get; set; }
    public int Quantity { get; set; }
}

public class PlaceOrderResponse
{
    public string OrderNumber { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
}

public class GetOrderResponse
{
    public string OrderNumber { get; set; } = string.Empty;
    public long ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = string.Empty;
}
