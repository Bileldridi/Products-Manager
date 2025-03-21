using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Products.DbContexts;
using Products.Entities;
using System.Security.Claims;

namespace Products.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/cart")]
    public class CartController : ControllerBase
    {
        private readonly ProductContext _db;

        public CartController(ProductContext db)
        {
            _db = db;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] List<CartItem> cartItems)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var cart = await _db.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId, Items = new List<CartItem>() };
                _db.Carts.Add(cart);
            }

            foreach (var item in cartItems)
            {
                var existingItem = cart.Items.FirstOrDefault(ci => ci.ProductId == item.ProductId);
                if (existingItem != null)
                {
                    existingItem.Quantity += item.Quantity;
                }
                else
                {
                    cart.Items.Add(item);
                }
            }

            await _db.SaveChangesAsync();
            return Ok("Products added to cart");
        }

        [HttpPost("remove")]
        public async Task<IActionResult> RemoveFromCart([FromBody] List<int> productIds)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var cart = await _db.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null) return NotFound("Cart not found");

            cart.Items.RemoveAll(ci => productIds.Contains(ci.ProductId));
            await _db.SaveChangesAsync();
            return Ok("Products removed from cart");
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var cart = await _db.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == userId);
            return cart != null ? Ok(cart) : NotFound("Cart not found");
        }
    }
}
