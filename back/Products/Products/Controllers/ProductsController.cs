using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Products.DbContexts;
using Products.Entities;
using Products.Models;
using System.Security.Claims;

namespace Products.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductContext _db;

        public ProductsController(ProductContext db)
        {
            _db = db;
        }
        private bool IsAdmin()
        {
            return User.Identity.IsAuthenticated && User.Claims.Any(c => c.Type == ClaimTypes.Email && c.Value == "admin@admin.com");
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ProductDto productDto)
        {
            if (!IsAdmin()) return Forbid();
            var product = new Product
            {
                Code = productDto.Code,
                Name = productDto.Name,
                Description = productDto.Description,
                Image = productDto.Image,
                Category = productDto.Category,
                Price = productDto.Price,
                Quantity = productDto.Quantity,
                InternalReference = productDto.InternalReference,
                ShellId = productDto.ShellId,
                InventoryStatus = productDto.InventoryStatus,
                Rating = productDto.Rating,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _db.Products.Add(product);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _db.Products.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProductById(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return NotFound();
            return product;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductDto updateProductDto)
        {
            if (!IsAdmin()) return Forbid();
            var product = await _db.Products.FindAsync(id);
            if (product == null) return NotFound();

            product.Code = updateProductDto.Code;
            product.Name = updateProductDto.Name;
            product.Description = updateProductDto.Description;
            product.Image = updateProductDto.Image;
            product.Category = updateProductDto.Category;
            product.Price = updateProductDto.Price;
            product.Quantity = updateProductDto.Quantity;
            product.InternalReference = updateProductDto.InternalReference;
            product.ShellId = updateProductDto.ShellId;
            product.InventoryStatus = updateProductDto.InventoryStatus;
            product.Rating = updateProductDto.Rating;
            product.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchProduct(int id, [FromBody] JsonPatchDocument<Product> patchDoc)
        {
            if (!IsAdmin()) return Forbid();
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var product = await _db.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            patchDoc.ApplyTo(product, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            product.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return Ok(product);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (!IsAdmin()) return Forbid();
            var product = await _db.Products.FindAsync(id);
            if (product == null) return NotFound();

            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
