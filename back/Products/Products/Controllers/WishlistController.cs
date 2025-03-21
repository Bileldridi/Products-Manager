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
    [Route("api/wishlist")]
    public class WishlistController : ControllerBase
    {
        private readonly ProductContext _db;

        public WishlistController(ProductContext db)
        {
            _db = db;
        }

        [HttpPost("toggle")]
        public async Task<IActionResult> ToggleWishlistItem([FromBody] int productId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var wishlist = await _db.Wishlists.Include(w => w.Items).FirstOrDefaultAsync(w => w.UserId == userId);

            if (wishlist == null)
            {
                wishlist = new Wishlist { UserId = userId, Items = new List<WishlistItem>() };
                _db.Wishlists.Add(wishlist);
            }

            var existingItem = wishlist.Items.FirstOrDefault(wi => wi.ProductId == productId);
            if (existingItem != null)
            {
                wishlist.Items.Remove(existingItem);
            }
            else
            {
                wishlist.Items.Add(new WishlistItem { ProductId = productId });
            }

            await _db.SaveChangesAsync();
            return Ok("Wishlist updated");
        }

        [HttpGet]
        public async Task<IActionResult> GetWishlist()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var wishlist = await _db.Wishlists.Include(w => w.Items).FirstOrDefaultAsync(w => w.UserId == userId);
            return wishlist != null ? Ok(wishlist) : NotFound("Wishlist not found");
        }
    }
}
