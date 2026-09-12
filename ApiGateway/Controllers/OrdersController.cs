using Microsoft.AspNetCore.Mvc;
using System.Text;
using ApiGateway.DTOs;
using System.Net.Http.Json;

namespace ApiGateway.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly HttpClient _httpClient;

    public OrdersController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("OrderService");
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var response = await _httpClient.GetAsync("orders");
        var content = await response.Content.ReadAsStringAsync();

        return new ContentResult
        {
            StatusCode = (int)response.StatusCode,
            Content = content,
            ContentType = "application/json"
        };
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var response = await _httpClient.GetAsync($"orders/{id}");

        var content = await response.Content.ReadAsStringAsync();

        return new ContentResult
        {
            StatusCode = (int)response.StatusCode,
            Content = content,
            ContentType = "application/json"
        };
    }

    [HttpPost]
public async Task<IActionResult> Create(CreateOrderRequest request)
{
    var response = await _httpClient.PostAsJsonAsync(
        "orders",
        request);

    var content = await response.Content.ReadAsStringAsync();

    return new ContentResult
    {
        StatusCode = (int)response.StatusCode,
        Content = content,
        ContentType = "application/json"
    };
}
}