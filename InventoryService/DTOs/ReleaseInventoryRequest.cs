namespace InventoryService.DTOs;

public class ReleaseInventoryRequest
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}