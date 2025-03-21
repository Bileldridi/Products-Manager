using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Products.Controllers;
using Products.DbContexts;
using Products.Entities;
using Products.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Products.Enums;

namespace Products.Tests
{
    public class ProductsControllerTests
    {
        private readonly ProductContext _dbContext;
        private readonly ProductsController _productsController;

        public ProductsControllerTests()
        {
            var options = new DbContextOptionsBuilder<ProductContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;
            _dbContext = new ProductContext(options);
            _productsController = new ProductsController(_dbContext);
        }

        private void SetUserClaims(string email, bool isAuthenticated = true)
        {
            var mockUser = new Mock<ClaimsPrincipal>();
            mockUser.Setup(u => u.Identity.IsAuthenticated).Returns(isAuthenticated);
            mockUser.Setup(u => u.Claims).Returns(new List<Claim>
        {
            new Claim(ClaimTypes.Email, email)
        });

            _productsController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = mockUser.Object }
            };
        }

        [Fact]
        public async Task CreateProduct_ShouldReturnForbid_IfNotAdmin()
        {
            // Arrange
            var productDto = new ProductDto
            {
                Code = "P001",
                Name = "Product 1",
                Description = "Description",
                Price = 100,
                Quantity = 10
            };

            SetUserClaims("user@user.com");

            // Act
            var result = await _productsController.CreateProduct(productDto);

            // Assert
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task GetProducts_ShouldReturnEmptyList_WhenNoProducts()
        {
            var result = await _productsController.GetProducts();

            Assert.Empty(result.Value);
        }

        [Fact]
        public async Task DeleteProduct_ShouldReturnForbid_IfNotAdmin()
        {
            // Arrange
            SetUserClaims("user@user.com");

            // Act
            var result = await _productsController.DeleteProduct(1);

            // Assert
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task DeleteProduct_ShouldReturnNoContent_IfAdminAndProductExists()
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

            SetUserClaims("admin@admin.com");

            // Act
            var result = await _productsController.DeleteProduct(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteProduct_ShouldReturnNotFound_IfAdminAndProductDoesNotExist()
        {
            // Arrange
            SetUserClaims("admin@admin.com");

            // Act
            var result = await _productsController.DeleteProduct(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}

