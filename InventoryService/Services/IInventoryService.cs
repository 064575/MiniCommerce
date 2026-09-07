using InventoryService.DTOs;

namespace InventoryService.Services;

public interface IInventoryService
{
    Task<List<InventoryItemDto>> GetAllAsync();
    Task<InventoryItemDto?> GetByProductIdAsync(Guid productId);

    Task<InventoryItemDto> AddStockAsync(AddStockRequest request);
    Task<InventoryItemDto> ReserveAsync(ReserveInventoryRequest request);
    Task<InventoryItemDto> ReleaseAsync(ReleaseInventoryRequest request);
    Task<InventoryItemDto> ConfirmAsync(ConfirmInventoryRequest request);
}