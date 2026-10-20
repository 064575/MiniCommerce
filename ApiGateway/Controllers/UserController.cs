using Microsoft.AspNetCore.Mvc;
using ApiGateway.DTOs;
using System.Net.Http.Json;

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

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "users/login",
            request);

        var content = await response.Content.ReadAsStringAsync();

        return new ContentResult
        {
            StatusCode = (int)response.StatusCode,
            Content = content,
            ContentType = "application/json"
        };
    }

    [HttpPost]
    public async Task<IActionResult> Register(CreateUserRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "users",
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