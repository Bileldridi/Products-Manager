using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Products.Controllers;
using Products.DbContexts;
using Products.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Products.Tests
{
    public class CartControllerTests
    {
        private readonly ProductContext _context;
        private readonly CartController _controller;

        public CartControllerTests()
        {
            var options = new DbContextOptionsBuilder<ProductContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new ProductContext(options);
            _controller = new CartController(_context);
        }

        [Fact]
        public async Task AddToCart_ShouldAddItemsToCart()
        {
            // Arrange
            var userId = 1;
            var cartItems = new List<CartItem>
            {
                new CartItem { ProductId = 1, Quantity = 2 },
                new CartItem { ProductId = 2, Quantity = 3 }
            };

            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };
            var identity = new ClaimsIdentity(userClaims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            var cart = new Cart { UserId = userId, Items = new List<CartItem>() };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.AddToCart(cartItems);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Products added to cart", okResult.Value);
            Assert.Equal(2, cart.Items.Count);
            Assert.Equal(2, cart.Items.First(ci => ci.ProductId == 1).Quantity);
            Assert.Equal(3, cart.Items.First(ci => ci.ProductId == 2).Quantity);
        }

        [Fact]
        public async Task GetCart_ShouldReturnNotFound_WhenCartIsEmpty()
        {
            // Arrange
            var userId = 1;
            var userClaims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, userId.ToString())
    };
            var identity = new ClaimsIdentity(userClaims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Act
            var result = await _controller.GetCart();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Cart not found", notFoundResult.Value);
        }

    }
}
