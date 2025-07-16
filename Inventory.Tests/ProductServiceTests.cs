using Moq;
using Inventory.Core.Entities;
using Inventory.Core.Interfaces;
using Inventory.Application.Services; 

namespace Inventory.Tests
{
    public class ProductServiceTests
    {
        [Fact]
        public void AddProduct_ShouldCallRepositoryWithCorrectProduct()
        {
            // Arrange
            var mockRepo = new Mock<IProductRepository>();
            var productService = new ProductService(mockRepo.Object);

            var testProduct = new Product
            {
                Id = 1,
                Name = "Test Product",
                Description = "Sample product",
                Price = 99.99m
            };

            // Act
            productService.AddProduct(testProduct);

            // Assert
            mockRepo.Verify(repo => repo.AddAsync(It.Is<Product>(
                p => p.Id == testProduct.Id &&
                     p.Name == testProduct.Name &&
                     p.Description == testProduct.Description &&
                     p.Price == testProduct.Price
            )), Times.Once);
        }

        [Fact]
        public async Task AddProduct_ShouldCallRepository_WithCorrectProduct()
        {
            // Arrange
            var mockRepo = new Mock<IProductRepository>();
            var service = new ProductService(mockRepo.Object);
            var product = new Product { Id = 1, Name = "Sample Product" };

            // Act
            await service.AddProductAsync(product);

            // Assert
            mockRepo.Verify(r => r.AddAsync(product), Times.Once);
        }

    }
}
