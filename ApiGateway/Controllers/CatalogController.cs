using ApiGateway.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace ApiGateway.Controllers;

[ApiController]
[Route("api/catalog")]
public class CatalogController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    public CatalogController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CatalogProductDto>>> GetAll()
    {
        var productClient =
            _httpClientFactory.CreateClient("ProductService");

        var inventoryClient =
            _httpClientFactory.CreateClient("InventoryService");

        var products =
            await productClient.GetFromJsonAsync<List<ProductDto>>("products")
            ?? new List<ProductDto>();

        var inventory =
            await inventoryClient.GetFromJsonAsync<List<InventoryItemDto>>("inventory")
            ?? new List<InventoryItemDto>();

        var catalog = products
            .Where(product => product.IsActive)
            .Select(product =>
            {
                var inventoryItem = inventory
                    .FirstOrDefault(item => item.ProductId == product.Id);

                return new CatalogProductDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    IsAvailable =
                        inventoryItem != null &&
                        inventoryItem.AvailableQuantity > 0
                };
            })
            .ToList();

        return Ok(catalog);
    }
}