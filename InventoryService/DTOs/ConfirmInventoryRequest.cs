namespace InventoryService.DTOs;

public class ConfirmInventoryRequest
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}