using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Products.Controllers;
using Products.DbContexts;
using Products.Entities;
using Products.Enums;
using Products.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Products.IntegrationTests
{
    public class ProductsControllerIntegrationTests
    {
        private readonly ProductContext _dbContext;
        private readonly ProductsController _productsController;

        public ProductsControllerIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<ProductContext>()
                .UseInMemoryDatabase(databaseName: "IntegrationTestDb")
                .Options;
            _dbContext = new ProductContext(options);
            _productsController = new ProductsController(_dbContext);
            // Mock the User and User.Identity
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Email, "admin@admin.com")
            }, "mock"));

            var httpContext = new DefaultHttpContext { User = user };
            _productsController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        [Fact]
        public async Task CreateProduct_ShouldReturnCreatedProduct()
        {
            // Arrange
            var productDto = new ProductDto
            {
                Code = "P001",
                Name = "Product 1",
                Description = "Description",
                Image = "image_url",
                Category = "Category 1",
                Price = 100,
                Quantity = 10,
                InternalReference = "Ref001",
                ShellId = 1,
                InventoryStatus = InventoryStatus.INSTOCK,
                Rating = 5
            };

            // Act
            var result = await _productsController.CreateProduct(productDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var createdProduct = Assert.IsType<Product>(createdResult.Value);
            Assert.Equal(productDto.Code, createdProduct.Code);
            Assert.Equal(productDto.Name, createdProduct.Name);
            Assert.Equal(productDto.Description, createdProduct.Description);
            Assert.Equal(productDto.Image, createdProduct.Image);
            Assert.Equal(productDto.Category, createdProduct.Category);
            Assert.Equal(productDto.Price, createdProduct.Price);
            Assert.Equal(productDto.Quantity, createdProduct.Quantity);
            Assert.Equal(productDto.InternalReference, createdProduct.InternalReference);
            Assert.Equal(productDto.ShellId, createdProduct.ShellId);
            Assert.Equal(productDto.InventoryStatus, createdProduct.InventoryStatus);
            Assert.Equal(productDto.Rating, createdProduct.Rating);
        }

        [Fact]
        public async Task GetProductById_ShouldReturnProduct()
        {
            // Arrange
            var product = new Product
            {
                Code = "P001",
                Name = "Product 1",
                Description = "Description",
                Image = "image_url",
                Category = "Category 1",
                Price = 100,
                Quantity = 10,
                InternalReference = "Ref001",
                ShellId = 1,
                InventoryStatus = InventoryStatus.INSTOCK,
                Rating = 5
            };
            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _productsController.GetProductById(product.Id);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Product>>(result);
            var returnValue = Assert.IsType<Product>(actionResult.Value);
            Assert.Equal(product.Id, returnValue.Id);
        }

        [Fact]
        public async Task UpdateProduct_ShouldReturnNoContent()
        {
            // Arrange
            var product = new Product
            {
                Code = "P001",
                Name = "Product 1",
                Description = "Description",
                Image = "image_url",
                Category = "Category 1",
                Price = 100,
                Quantity = 10,
                InternalReference = "Ref001",
                ShellId = 1,
                InventoryStatus = InventoryStatus.INSTOCK,
                Rating = 5
            };
            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync();

            var updateProductDto = new ProductDto
            {
                Code = "P002",
                Name = "Updated Product",
                Description = "Updated Description",
                Image = "updated_image_url",
                Category = "Updated Category",
                Price = 200,
                Quantity = 20,
                InternalReference = "Ref002",
                ShellId = 2,
                InventoryStatus = InventoryStatus.OUTOFSTOCK,
                Rating = 4
            };

            // Act
            var result = await _productsController.UpdateProduct(product.Id, updateProductDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
            var updatedProduct = await _dbContext.Products.FindAsync(product.Id);
            Assert.Equal(updateProductDto.Code, updatedProduct.Code);
            Assert.Equal(updateProductDto.Name, updatedProduct.Name);
            Assert.Equal(updateProductDto.Description, updatedProduct.Description);
            Assert.Equal(updateProductDto.Image, updatedProduct.Image);
            Assert.Equal(updateProductDto.Category, updatedProduct.Category);
            Assert.Equal(updateProductDto.Price, updatedProduct.Price);
            Assert.Equal(updateProductDto.Quantity, updatedProduct.Quantity);
            Assert.Equal(updateProductDto.InternalReference, updatedProduct.InternalReference);
            Assert.Equal(updateProductDto.ShellId, updatedProduct.ShellId);
            Assert.Equal(updateProductDto.InventoryStatus, updatedProduct.InventoryStatus);
            Assert.Equal(updateProductDto.Rating, updatedProduct.Rating);
        }

        [Fact]
        public async Task DeleteProduct_ShouldReturnNoContent()
        {
            // Arrange
            var product = new Product
            {
                Code = "P001",
                Name = "Product 1",
                Description = "Description",
                Image = "image_url",
                Category = "Category 1",
                Price = 100,
                Quantity = 10,
                InternalReference = "Ref001",
                ShellId = 1,
                InventoryStatus = InventoryStatus.INSTOCK,
                Rating = 5
            };
            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _productsController.DeleteProduct(product.Id);

            // Assert
            Assert.IsType<NoContentResult>(result);
            var deletedProduct = await _dbContext.Products.FindAsync(product.Id);
            Assert.Null(deletedProduct);
        }
    }
}
