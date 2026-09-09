namespace OrderService.Clients;

public interface IInventoryClient
{
    Task ReserveAsync(Guid productId, int quantity);
    Task ReleaseAsync(Guid productId, int quantity);
    Task ConfirmAsync(Guid productId, int quantity);
}