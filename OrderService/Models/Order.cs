namespace OrderService.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }

        public List<OrderItem> Items { get; set; } = new();
    }
}
