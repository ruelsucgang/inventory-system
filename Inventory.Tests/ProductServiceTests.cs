using Moq;
using Inventory.Core.Entities;
using Inventory.Core.Interfaces;
using Inventory.Application.Services; 

namespace Inventory.Tests
{
    public class ProductServiceTests
    {
        [Fact]
        public async Task AddProductAsync_ShouldCallRepository_WithCorrectProduct()
        {
            // Arrange
            var mockRepo = new Mock<IProductRepository>();
            var mockCache = new Mock<ICacheService>();
            var productService = new ProductService(mockRepo.Object, mockCache.Object);
            var testProduct = new Product
            {
                Id = 1,
                Name = "Test Product",
                Description = "Sample",
                Price = 99.99m
            };

            // Act
            await productService.AddProductAsync(testProduct);

            // Assert
            mockRepo.Verify(repo => repo.AddAsync(It.Is<Product>(
                p => p.Id == testProduct.Id &&
                     p.Name == testProduct.Name &&
                     p.Description == testProduct.Description &&
                     p.Price == testProduct.Price
            )), Times.Once);

            mockCache.Verify(c => c.RemoveAsync("product_list"), Times.Once);
        }

        [Fact]
        public async Task GetAllProductsAsync_ShouldReturnCachedData_IfAvailable()
        {
            // Arrange
            var mockRepo = new Mock<IProductRepository>();
            var mockCache = new Mock<ICacheService>();
            var cachedList = new List<Product> { new Product { Id = 1, Name = "Cached Product" } };

            mockCache.Setup(c => c.GetAsync<IEnumerable<Product>>("product_list"))
                     .ReturnsAsync(cachedList);

            var service = new ProductService(mockRepo.Object, mockCache.Object);

            // Act
            var result = await service.GetAllProductsAsync();

            // Assert
            Assert.Equal(cachedList, result);
            mockRepo.Verify(r => r.GetAllAsync(), Times.Never);
        }

        [Fact]
        public async Task GetProductByIdAsync_ShouldReturnProduct()
        {
            var mockRepo = new Mock<IProductRepository>();
            var mockCache = new Mock<ICacheService>();
            var service = new ProductService(mockRepo.Object, mockCache.Object);

            var product = new Product { Id = 1, Name = "Test" };
            mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

            var result = await service.GetProductByIdAsync(1);

            Assert.Equal(product, result);
        }

        [Fact]
        public async Task UpdateProductAsync_ShouldReturnFalse_IfNotFound()
        {
            var mockRepo = new Mock<IProductRepository>();
            var mockCache = new Mock<ICacheService>();
            var service = new ProductService(mockRepo.Object, mockCache.Object);

            mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Product?)null);

            var result = await service.UpdateProductAsync(new Product { Id = 1 });

            Assert.False(result);
        }

        [Fact]
        public async Task DeleteProductAsync_ShouldReturnFalse_IfNotFound()
        {
            var mockRepo = new Mock<IProductRepository>();
            var mockCache = new Mock<ICacheService>();
            var service = new ProductService(mockRepo.Object, mockCache.Object);

            mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Product?)null);

            var result = await service.DeleteProductAsync(999);

            Assert.False(result);
        }


    }
}
