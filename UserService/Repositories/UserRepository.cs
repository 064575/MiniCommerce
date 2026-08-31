using System.Text.Json;
using UserService.Models;

namespace UserService.Repositories;

public class UserRepository : IUserRepository
{
    private readonly string _filePath;

    public UserRepository(IWebHostEnvironment environment)
    {
        _filePath = Path.Combine(
            environment.ContentRootPath,
            "Storage",
            "users.json"
        );
    }

    public async Task<List<User>> GetAllAsync()
    {
        if (!File.Exists(_filePath))
        {
            return new List<User>();
        }

        var json = await File.ReadAllTextAsync(_filePath);

        if (string.IsNullOrWhiteSpace(json))
        {
            return new List<User>();
        }

        return JsonSerializer.Deserialize<List<User>>(json)
               ?? new List<User>();
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        var users = await GetAllAsync();

        return users.FirstOrDefault(x => x.Id == id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var users = await GetAllAsync();

        return users.FirstOrDefault(x =>
            x.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
    }

    public async Task AddAsync(User user)
    {
        var users = await GetAllAsync();

        users.Add(user);

        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        var json = JsonSerializer.Serialize(users, options);

        await File.WriteAllTextAsync(_filePath, json);
    }
}