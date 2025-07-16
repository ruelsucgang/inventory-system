using Inventory.Core.Entities;
using Inventory.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductsController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [AllowAnonymous]  // this is excempted for token validation
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var products = await _productRepository.GetAllAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"ERROR: {ex.Message} | INNER: {ex.InnerException?.Message}");
            }

            //        var sampleProducts = new List<object>
            //{
            //    new { Id = 1, Name = "Sample Product 1", Price = 10.99 },
            //    new { Id = 2, Name = "Sample Product 2", Price = 25.50 },
            //    new { Id = 3, Name = "Sample Product 3", Price = 99.00 }
            //};

            //    return Ok(sampleProducts);
        }

        [HttpGet("{id}", Name = "GetProductById")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            var created = await _productRepository.AddAsync(product);
            return CreatedAtRoute("GetProductById", new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Product product)
        {
            if (id != product.Id)
                return BadRequest();

            var existing = await _productRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            await _productRepository.UpdateAsync(product);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            var result = await _productRepository.DeleteAsync(id);
            if (!result)
                return StatusCode(500, "An error occurred while deleting the product.");

            return NoContent();
        }
    }
}
