using Inventory.Core.Entities;

namespace Inventory.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> RegisterAsync(User user);
        Task<string?> LoginAsync(string username, string password);
        User? GetById(int id);
        void Update(User user);
    }

}
