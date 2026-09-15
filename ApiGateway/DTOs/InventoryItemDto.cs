namespace ApiGateway.DTOs;

public class InventoryItemDto
{
    public Guid ProductId { get; set; }
    public int AvailableQuantity { get; set; }
    public int ReservedQuantity { get; set; }
}