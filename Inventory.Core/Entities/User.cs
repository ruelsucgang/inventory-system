
namespace Inventory.Core.Entities
{
    public class User
    {
        public int Id { get; set; }  // primary key
        public string Username { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
        public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();
        public string Role { get; set; } = "User";
    }
}
