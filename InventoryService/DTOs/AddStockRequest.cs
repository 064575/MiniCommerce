namespace InventoryService.DTOs;

public class AddStockRequest
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}