namespace InventoryService.DTOs;

public class ReserveInventoryRequest
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}