using Inventory.Core.Entities;
using Inventory.Core.Interfaces;

namespace Inventory.Application.Services
{
    public class ProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public void AddProduct(Product product)
        {
            _productRepository.AddAsync(product);
        }

        public async Task AddProductAsync(Product product)
        {
            await _productRepository.AddAsync(product);
        }
    }
}
