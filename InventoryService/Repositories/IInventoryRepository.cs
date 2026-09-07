using InventoryService.Models;

namespace InventoryService.Repositories;

public interface IInventoryRepository
{
    Task<List<InventoryItem>> GetAllAsync();
    Task<InventoryItem?> GetByProductIdAsync(Guid productId);
    Task SaveAsync(InventoryItem item);
}