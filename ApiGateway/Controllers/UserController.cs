using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly HttpClient _httpClient;

    public UsersController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("UserService");
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var response = await _httpClient.GetAsync("users");

        var content = await response.Content.ReadAsStringAsync();

        return new ContentResult
        {
            StatusCode = (int)response.StatusCode,
            Content = content,
            ContentType = "application/json"
        };
    }
}