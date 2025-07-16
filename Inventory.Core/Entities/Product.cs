using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Core.Entities
{
    public class Product
    {
        public int Id { get; set; } // Primary Key
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.UtcNow;
    }
}
