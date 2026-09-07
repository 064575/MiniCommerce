namespace InventoryService.Models
{
    public class InventoryItem
    {
        public Guid ProductId { get; set; }
        public int AvailableQuantity { get; set; }
        public int ReservedQuantity { get; set; }
    }
}
