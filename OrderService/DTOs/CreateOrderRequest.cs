namespace OrderService.DTOs;

public class CreateOrderRequest
{
    public Guid UserId { get; set; }
    public List<CreateOrderItemRequest> Items { get; set; } = new();
}