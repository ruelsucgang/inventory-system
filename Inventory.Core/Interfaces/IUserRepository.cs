using Inventory.Core.Entities;

namespace Inventory.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> RegisterAsync(User user);
        Task<string?> LoginAsync(string username, string password);
        Task<User?> GetByIdAsync(int id); 
        Task<User?> GetByUsernameAsync(string username);
        Task<bool> UserExistsAsync(string username);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
    }
}

