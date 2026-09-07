using System.Text.Json;
using InventoryService.Models;

namespace InventoryService.Repositories;

public class InventoryRepository : IInventoryRepository
{
    private readonly string _filePath;

    public InventoryRepository(IWebHostEnvironment environment)
    {
        _filePath = Path.Combine(
            environment.ContentRootPath,
            "Storage",
            "inventory.json"
        );
    }

    public async Task<List<InventoryItem>> GetAllAsync()
    {
        if (!File.Exists(_filePath))
        {
            return new List<InventoryItem>();
        }

        var json = await File.ReadAllTextAsync(_filePath);

        if (string.IsNullOrWhiteSpace(json))
        {
            return new List<InventoryItem>();
        }

        return JsonSerializer.Deserialize<List<InventoryItem>>(json)
               ?? new List<InventoryItem>();
    }

    public async Task<InventoryItem?> GetByProductIdAsync(Guid productId)
    {
        var items = await GetAllAsync();

        return items.FirstOrDefault(x => x.ProductId == productId);
    }

    public async Task SaveAsync(InventoryItem item)
    {
        var items = await GetAllAsync();

        var existingItem = items.FirstOrDefault(x => x.ProductId == item.ProductId);

        if (existingItem is null)
        {
            items.Add(item);
        }
        else
        {
            existingItem.AvailableQuantity = item.AvailableQuantity;
            existingItem.ReservedQuantity = item.ReservedQuantity;
        }

        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        var json = JsonSerializer.Serialize(items, options);

        await File.WriteAllTextAsync(_filePath, json);
    }
}