using Inventory.Core.Entities;
using Inventory.Core.Interfaces;
using Inventory.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Inventory.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> RegisterAsync(User user)
        {
            if (await _context.Users.AnyAsync(u => u.Username == user.Username))
                return false;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<string?> LoginAsync(string username, string password)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (existingUser == null) return null;

            using var hmac = new HMACSHA512(existingUser.PasswordSalt);
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            if (!computedHash.SequenceEqual(existingUser.PasswordHash))
                return null;

            return "test-token-or-service-calls-for-now";
        }

        public User? GetById(int id)
        {
            return _context.Users.FirstOrDefault(u => u.Id == id);
        }

        public void Update(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
        }
    }
}
