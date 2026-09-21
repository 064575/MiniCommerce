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

        var orderItems = new List<OrderItem>();

        foreach (var item in request.Items)
        {
            if (item.ProductId == Guid.Empty)
            {
                throw new ArgumentException("ProductId is required.");
            }

            if (item.Quantity <= 0)
            {
                throw new ArgumentException(
                    "Quantity must be greater than zero.");
            }

            var product = await _productClient.GetByIdAsync(item.ProductId);

            if (product is null)
            {
                throw new InvalidOperationException(
                    $"Product {item.ProductId} does not exist.");
            }

            if (!product.IsActive)
            {
                throw new InvalidOperationException(
                    $"Product {item.ProductId} is not active.");
            }

            orderItems.Add(new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = product.Price
            });
        }

        var reservedItems = new List<CreateOrderItemRequest>();

        // 1. Prvo rezervišemo sve stavke.
        // Ako neka rezervacija padne, vraćamo prethodno rezervisane.
        try
        {
            foreach (var item in request.Items)
            {
                await _inventoryClient.ReserveAsync(
                    item.ProductId,
                    item.Quantity);

                reservedItems.Add(item);
            }
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
                }
            }

            throw;
        }

        // 2. Sve je rezervisano - kreiramo Pending order.
        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            CreatedAt = DateTime.UtcNow,
            Status = "Pending",
            Items = orderItems,
            TotalPrice = orderItems.Sum(
                item => item.UnitPrice * item.Quantity)
        };

        await _orderRepository.AddAsync(order);

        // 3. Potvrđujemo rezervacije.
        try
        {
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
            // Order je već napravljen, zato ga ne ostavljamo kao Pending.
            order.Status = "Failed";
            await _orderRepository.UpdateAsync(order);

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
            TotalPrice = order.TotalPrice,
            Items = order.Items.Select(item => new OrderItemDto
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToList()
        };
    }
}