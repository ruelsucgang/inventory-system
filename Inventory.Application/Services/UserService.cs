using Inventory.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public bool ChangePassword(int userId, string oldPassword, string newPassword)
        {
            var user = _userRepository.GetById(userId);
            if (user == null) return false;

            using var hmac = new System.Security.Cryptography.HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(oldPassword));

            if (!computedHash.SequenceEqual(user.PasswordHash))
                return false;

            // Generate new hash/salt
            using var newHmac = new System.Security.Cryptography.HMACSHA512();
            user.PasswordSalt = newHmac.Key;
            user.PasswordHash = newHmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(newPassword));

            _userRepository.Update(user);
            return true;
        }

    }
}
