using UserService.Models;

namespace UserService.Repositories;

public interface IUserRepository
{
    Task<List<User>> GetAllAsync();
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(String email);
    Task AddAsync(User user);

}