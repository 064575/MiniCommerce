using OrderService.DTOs;
using System.Net;
using System.Net.Http.Json;

namespace OrderService.Clients;

public class ProductClient : IProductClient
{
    private readonly HttpClient _httpClient;

    public ProductClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }


    public async Task<ProductDto?> GetByIdAsync(Guid productId)
    {
        var response = await _httpClient.GetAsync(
            $"products/{productId}"
        );

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content
            .ReadFromJsonAsync<ProductDto>();
    }

}