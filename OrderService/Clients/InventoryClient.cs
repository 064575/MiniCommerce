using System.Net.Http.Json;
using System.Net;

namespace OrderService.Clients;

public class InventoryClient : IInventoryClient
{
    private readonly HttpClient _httpClient;

    public InventoryClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task ReserveAsync(Guid productId, int quantity)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "inventory/reserve",
            new
            {
                productId,
                quantity
            });

        if (response.StatusCode == HttpStatusCode.Conflict)
        {
            throw new InvalidOperationException(
                "Not enough inventory available for this product.");
        }

        response.EnsureSuccessStatusCode();
    }

    public async Task ReleaseAsync(Guid productId, int quantity)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "inventory/release",
            new
            {
                productId,
                quantity
            });

        response.EnsureSuccessStatusCode();
    }

    public async Task ConfirmAsync(Guid productId, int quantity)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "inventory/confirm",
            new
            {
                productId,
                quantity
            });

        response.EnsureSuccessStatusCode();
    }
}