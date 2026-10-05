using Microsoft.AspNetCore.Mvc;
using OrderService.DTOs;
using OrderService.Services;

namespace OrderService.Controllers;

[ApiController]
[Route("orders")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<ActionResult<List<OrderDto>>> GetAll()
    {
        var orders = await _orderService.GetAllAsync();
        return Ok(orders);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderDto>> GetById(Guid id)
    {
        var order = await _orderService.GetByIdAsync(id);

        if (order is null)
        {
            return NotFound();
        }

        return Ok(order);
    }

    [HttpGet("{id:guid}/user/{userId:guid}")]
    public async Task<ActionResult<OrderDto>> GetByIdAndUserId(
    Guid id,
    Guid userId)
    {
        var order = await _orderService.GetByIdAndUserIdAsync(
            id,
            userId);

        if (order is null)
        {
            return NotFound();
        }

        return Ok(order);
    }


    [HttpGet("user/{userId:guid}")]
    public async Task<ActionResult<List<OrderDto>>> GetByUserId(Guid userId)
    {
        var orders = await _orderService.GetByUserIdAsync(userId);

        return Ok(orders);
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create(CreateOrderRequest request)
    {
        var order = await _orderService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = order.Id },
            order);
    }
}