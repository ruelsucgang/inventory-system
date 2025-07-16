using Inventory.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<User> Users => Set<User>();

    }
}
