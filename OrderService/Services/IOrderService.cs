using OrderService.DTOs;

namespace OrderService.Services;

public interface IOrderService
{
    Task<List<OrderDto>> GetAllAsync();
    Task<OrderDto?> GetByIdAsync(Guid id);
    Task<OrderDto> CreateAsync(CreateOrderRequest request);
    Task<List<OrderDto>> GetByUserIdAsync(Guid userId);
    Task<OrderDto?> GetByIdAndUserIdAsync(Guid id, Guid userId);
}