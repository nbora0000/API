using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryApi.Data;
using InventoryApi.Models;
using SharedLibrary.Events;

namespace InventoryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly InventoryDbContext _db;
        private readonly ILogger<InventoryController> _logger;

        public InventoryController(InventoryDbContext db, ILogger<InventoryController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [HttpPost("reserve")]
        public async Task<IActionResult> Reserve([FromBody] ReserveRequest request)
        {
            // naive reservation: find stock by product
            var item = await _db.Stock.FirstOrDefaultAsync(s => s.ProductId == request.ProductId);
            if (item == null || item.QuantityAvailable < request.Quantity)
            {
                return BadRequest("Insufficient stock");
            }

            item.QuantityAvailable -= request.Quantity;
            await _db.SaveChangesAsync();

            var ev = new SharedLibrary.Events.InventoryReservedEvent
            {
                ReservationId = Guid.NewGuid(),
                OrderId = request.OrderId,
                ProductId = request.ProductId,
                Quantity = request.Quantity
            };

            _logger.LogInformation("Inventory reserved: {Event}", ev);

            return Ok(ev);
        }
    }

    public class ReserveRequest
    {
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}