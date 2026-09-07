using InventoryService.DTOs;
using InventoryService.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Controllers;

[ApiController]
[Route("inventory")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpGet]
    public async Task<ActionResult<List<InventoryItemDto>>> GetAll()
    {
        var items = await _inventoryService.GetAllAsync();
        return Ok(items);
    }

    [HttpGet("{productId:guid}")]
    public async Task<ActionResult<InventoryItemDto>> GetByProductId(Guid productId)
    {
        var item = await _inventoryService.GetByProductIdAsync(productId);

        if (item is null)
        {
            return NotFound();
        }

        return Ok(item);
    }

    [HttpPost("stock")]
    public async Task<ActionResult<InventoryItemDto>> AddStock(AddStockRequest request)
    {
        var item = await _inventoryService.AddStockAsync(request);
        return Ok(item);
    }

    [HttpPost("reserve")]
    public async Task<ActionResult<InventoryItemDto>> Reserve(ReserveInventoryRequest request)
    {
        var item = await _inventoryService.ReserveAsync(request);
        return Ok(item);
    }

    [HttpPost("release")]
    public async Task<ActionResult<InventoryItemDto>> Release(ReleaseInventoryRequest request)
    {
        var item = await _inventoryService.ReleaseAsync(request);
        return Ok(item);
    }

    [HttpPost("confirm")]
    public async Task<ActionResult<InventoryItemDto>> Confirm(ConfirmInventoryRequest request)
    {
        var item = await _inventoryService.ConfirmAsync(request);
        return Ok(item);
    }
}