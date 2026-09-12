using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly HttpClient _httpClient;

    public ProductsController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("ProductService");
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var response = await _httpClient.GetAsync("products");

        var content = await response.Content.ReadAsStringAsync();

        return new ContentResult
        {
            StatusCode = (int)response.StatusCode,
            Content = content,
            ContentType = "application/json"
        };
    }
}