using OrderService.Clients;
using OrderService.DTOs;
using OrderService.Models;
using OrderService.Repositories;

namespace OrderService.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUserClient _userClient;
    private readonly IProductClient _productClient;
    private readonly IInventoryClient _inventoryClient;

    public OrderService(
        IOrderRepository orderRepository,
        IUserClient userClient,
        IProductClient productClient,
        IInventoryClient inventoryClient)
    {
        _orderRepository = orderRepository;
        _userClient = userClient;
        _productClient = productClient;
        _inventoryClient = inventoryClient;
    }

    public async Task<List<OrderDto>> GetAllAsync()
    {
        var orders = await _orderRepository.GetAllAsync();

        return orders.Select(ToDto).ToList();
    }

    public async Task<OrderDto?> GetByIdAsync(Guid id)
    {
        var order = await _orderRepository.GetByIdAsync(id);

        return order is null ? null : ToDto(order);
    }

    public async Task<OrderDto> CreateAsync(CreateOrderRequest request)
    {
        if (request.UserId == Guid.Empty)
        {
            throw new ArgumentException("UserId is required.");
        }

        if (request.Items is null || request.Items.Count == 0)
        {
            throw new ArgumentException("Order must contain at least one item.");
        }

        foreach (var item in request.Items)
        {
            if (item.ProductId == Guid.Empty)
            {
                throw new ArgumentException("ProductId is required.");
            }

            if (item.Quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than 0.");
            }
        }

        var userExists = await _userClient.ExistsAsync(request.UserId);

        if (!userExists)
        {
            throw new InvalidOperationException("User does not exist.");
        }

        foreach (var item in request.Items)
        {
            var productExists = await _productClient.ExistsAsync(item.ProductId);

            if (!productExists)
            {
                throw new InvalidOperationException(
                    $"Product {item.ProductId} does not exist.");
            }
        }

        var reservedItems = new List<CreateOrderItemRequest>();

        try
        {
            foreach (var item in request.Items)
            {
                await _inventoryClient.ReserveAsync(
                    item.ProductId,
                    item.Quantity);

                reservedItems.Add(item);
            }

            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                CreatedAt = DateTime.UtcNow,
                Status = "Pending",
                Items = request.Items.Select(item => new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                }).ToList()
            };

            await _orderRepository.AddAsync(order);

            foreach (var item in request.Items)
            {
                await _inventoryClient.ConfirmAsync(
                    item.ProductId,
                    item.Quantity);
            }

            order.Status = "Completed";

            await _orderRepository.UpdateAsync(order);

            return ToDto(order);
        }
        catch
        {
            foreach (var item in reservedItems)
            {
                try
                {
                    await _inventoryClient.ReleaseAsync(
                        item.ProductId,
                        item.Quantity);
                }
                catch
                {
                    // Za sada samo pokušavamo rollback.
                    // Kasnije možemo dodati detaljniji logging.
                }
            }

            throw;
        }
    }

    private static OrderDto ToDto(Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            UserId = order.UserId,
            CreatedAt = order.CreatedAt,
            Status = order.Status,
            Items = order.Items.Select(item => new OrderItemDto
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity
            }).ToList()
        };
    }
}