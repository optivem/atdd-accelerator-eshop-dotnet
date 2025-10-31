namespace Optivem.AtddAccelerator.EShop.Monolith.Core.Entities;

public class Order
{
    public string OrderNumber { get; set; }
    public long ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public OrderStatus Status { get; set; }

    public Order(string orderNumber, long productId, int quantity, decimal unitPrice, decimal totalPrice, OrderStatus status)
    {
        if (orderNumber == null)
            throw new ArgumentNullException(nameof(orderNumber), "orderNumber cannot be null");
        if (unitPrice < 0)
            throw new ArgumentException("unitPrice cannot be negative", nameof(unitPrice));
        if (totalPrice < 0)
            throw new ArgumentException("totalPrice cannot be negative", nameof(totalPrice));

        OrderNumber = orderNumber;
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
        TotalPrice = totalPrice;
        Status = status;
    }
}
