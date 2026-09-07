using InventoryService.DTOs;
using InventoryService.Models;
using InventoryService.Repositories;

namespace InventoryService.Services;

public class InventoryService : IInventoryService
{
    private readonly IInventoryRepository _inventoryRepository;

    public InventoryService(IInventoryRepository inventoryRepository)
    {
        _inventoryRepository = inventoryRepository;
    }

    public async Task<List<InventoryItemDto>> GetAllAsync()
    {
        var items = await _inventoryRepository.GetAllAsync();

        return items.Select(ToDto).ToList();
    }

    public async Task<InventoryItemDto?> GetByProductIdAsync(Guid productId)
    {
        var item = await _inventoryRepository.GetByProductIdAsync(productId);

        if (item is null)
        {
            return null;
        }

        return ToDto(item);
    }

    public async Task<InventoryItemDto> AddStockAsync(AddStockRequest request)
    {
        ValidateQuantity(request.Quantity);

        var item = await _inventoryRepository.GetByProductIdAsync(request.ProductId);

        if (item is null)
        {
            item = new InventoryItem
            {
                ProductId = request.ProductId,
                AvailableQuantity = request.Quantity,
                ReservedQuantity = 0
            };
        }
        else
        {
            item.AvailableQuantity += request.Quantity;
        }

        await _inventoryRepository.SaveAsync(item);

        return ToDto(item);
    }

    public async Task<InventoryItemDto> ReserveAsync(ReserveInventoryRequest request)
    {
        ValidateQuantity(request.Quantity);

        var item = await _inventoryRepository.GetByProductIdAsync(request.ProductId);

        if (item is null)
        {
            throw new InvalidOperationException(
                "Inventory item does not exist.");
        }

        if (item.AvailableQuantity < request.Quantity)
        {
            throw new InvalidOperationException(
                "Not enough stock available.");
        }

        item.AvailableQuantity -= request.Quantity;
        item.ReservedQuantity += request.Quantity;

        await _inventoryRepository.SaveAsync(item);

        return ToDto(item);
    }

    public async Task<InventoryItemDto> ReleaseAsync(ReleaseInventoryRequest request)
    {
        ValidateQuantity(request.Quantity);

        var item = await _inventoryRepository.GetByProductIdAsync(request.ProductId);

        if (item is null)
        {
            throw new InvalidOperationException(
                "Inventory item does not exist.");
        }

        if (item.ReservedQuantity < request.Quantity)
        {
            throw new InvalidOperationException(
                "Cannot release more than reserved quantity.");
        }

        item.ReservedQuantity -= request.Quantity;
        item.AvailableQuantity += request.Quantity;

        await _inventoryRepository.SaveAsync(item);

        return ToDto(item);
    }

    public async Task<InventoryItemDto> ConfirmAsync(ConfirmInventoryRequest request)
    {
        ValidateQuantity(request.Quantity);

        var item = await _inventoryRepository.GetByProductIdAsync(request.ProductId);

        if (item is null)
        {
            throw new InvalidOperationException(
                "Inventory item does not exist.");
        }

        if (item.ReservedQuantity < request.Quantity)
        {
            throw new InvalidOperationException(
                "Cannot confirm more than reserved quantity.");
        }

        item.ReservedQuantity -= request.Quantity;

        await _inventoryRepository.SaveAsync(item);

        return ToDto(item);
    }

    private static void ValidateQuantity(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException(
                "Quantity must be greater than 0.");
        }
    }

    private static InventoryItemDto ToDto(InventoryItem item)
    {
        return new InventoryItemDto
        {
            ProductId = item.ProductId,
            AvailableQuantity = item.AvailableQuantity,
            ReservedQuantity = item.ReservedQuantity
        };
    }
}