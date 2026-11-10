namespace OrderService.Models
{
    public class OrderItem
    {
        public Guid ProductId { get; set; }

        public String ProductName { get; set; } = String.Empty;

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

    }
}
