using Inventory.Core.Entities;
using Inventory.Core.Interfaces;

namespace Inventory.Application.Services
{
    public class ProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICacheService _cacheService;
        private const string ProductCacheKey = "product_list";

        public ProductService(IProductRepository productRepository, ICacheService cacheService)
        {
            _productRepository = productRepository;
            _cacheService = cacheService;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            // Check Redis cache
            var cachedProducts = await _cacheService.GetAsync<IEnumerable<Product>>(ProductCacheKey);
            if (cachedProducts != null)
                return cachedProducts;

            // If not cached, fetch from DB
            var products = await _productRepository.GetAllAsync();

            // Store to Redis
            await _cacheService.SetAsync(ProductCacheKey, products, TimeSpan.FromMinutes(10));

            return products;
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _productRepository.GetByIdAsync(id);
        }

        public async Task<Product> AddProductAsync(Product product)
        {
            await _productRepository.AddAsync(product);
            // Optional: invalidate cache since data changed
            await _cacheService.RemoveAsync(ProductCacheKey);
            return product;
        }

        public async Task<bool> UpdateProductAsync(Product product)
        {
            var existing = await _productRepository.GetByIdAsync(product.Id);
            if (existing == null)
                return false;

            await _productRepository.UpdateAsync(product);
            await _cacheService.RemoveAsync(ProductCacheKey); // refresh cache next time
            return true;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return false;

            var result = await _productRepository.DeleteAsync(id);
            if (result)
                await _cacheService.RemoveAsync(ProductCacheKey); // refresh cache next time

            return result;
        }
    }
}
