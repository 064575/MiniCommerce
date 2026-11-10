namespace OrderService.DTOs;

public class OrderItemDto
{
    public Guid ProductId { get; set; }

    public String ProductName { get; set; } = String.Empty;

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

}