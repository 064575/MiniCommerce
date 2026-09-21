using OrderService.DTOs;

namespace OrderService.Clients;

public interface IProductClient
{
    Task<ProductDto?> GetByIdAsync(Guid productId);
}