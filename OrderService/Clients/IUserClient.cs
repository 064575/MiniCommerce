namespace OrderService.Clients;

public interface IUserClient
{
    Task<bool> ExistsAsync(Guid userId);
}