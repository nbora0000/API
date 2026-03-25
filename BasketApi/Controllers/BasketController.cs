using BasketApi.Data;
using BasketApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BasketApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BasketController : ControllerBase
{
    private readonly BasketDbContext _context;

    public BasketController(BasketDbContext context)
    {
        _context = context;
    }

    [HttpGet("{customerId}")]
    public async Task<ActionResult<ShoppingCart>> GetBasket(string customerId)
    {
        var basket = await _context.ShoppingCarts
            .Include(b => b.Items)
            .FirstOrDefaultAsync(b => b.CustomerId == customerId);

        if (basket == null)
            return new ShoppingCart { CustomerId = customerId };

        return basket;
    }

    [HttpPost]
    public async Task<ActionResult<ShoppingCart>> UpdateBasket(ShoppingCart basket)
    {
        var existingBasket = await _context.ShoppingCarts
            .Include(b => b.Items)
            .FirstOrDefaultAsync(b => b.CustomerId == basket.CustomerId);

        if (existingBasket == null)
        {
            _context.ShoppingCarts.Add(basket);
        }
        else
        {
            _context.CartItems.RemoveRange(existingBasket.Items);
            existingBasket.Items = basket.Items;
        }

        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBasket), new { customerId = basket.CustomerId }, basket);
    }

    [HttpDelete("{customerId}")]
    public async Task<IActionResult> DeleteBasket(string customerId)
    {
        var basket = await _context.ShoppingCarts.FindAsync(customerId);
        
        if (basket == null)
            return NotFound();

        _context.ShoppingCarts.Remove(basket);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
