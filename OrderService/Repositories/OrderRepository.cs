using System.Text.Json;
using OrderService.Models;

namespace OrderService.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly string _filePath;

    public OrderRepository(IWebHostEnvironment environment)
    {
        _filePath = Path.Combine(
            environment.ContentRootPath,
            "Storage",
            "orders.json"
        );
    }

    public async Task<List<Order>> GetAllAsync()
    {
        if (!File.Exists(_filePath))
        {
            return new List<Order>();
        }

        var json = await File.ReadAllTextAsync(_filePath);

        if (string.IsNullOrWhiteSpace(json))
        {
            return new List<Order>();
        }

        return JsonSerializer.Deserialize<List<Order>>(json)
               ?? new List<Order>();
    }

    public async Task<Order?> GetByIdAsync(Guid id)
    {
        var orders = await GetAllAsync();

        return orders.FirstOrDefault(x => x.Id == id);
    }

    public async Task AddAsync(Order order)
    {
        var orders = await GetAllAsync();

        orders.Add(order);

        await SaveAllAsync(orders);
    }

    public async Task UpdateAsync(Order order)
    {
        var orders = await GetAllAsync();

        var existingOrder = orders.FirstOrDefault(x => x.Id == order.Id);

        if (existingOrder is null)
        {
            return;
        }

        existingOrder.UserId = order.UserId;
        existingOrder.CreatedAt = order.CreatedAt;
        existingOrder.Status = order.Status;
        existingOrder.Items = order.Items;

        await SaveAllAsync(orders);
    }

    private async Task SaveAllAsync(List<Order> orders)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        var json = JsonSerializer.Serialize(orders, options);

        await File.WriteAllTextAsync(_filePath, json);
    }
}