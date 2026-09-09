namespace OrderService.Clients;

public interface IProductClient
{
    Task<bool> ExistsAsync(Guid productId);
}