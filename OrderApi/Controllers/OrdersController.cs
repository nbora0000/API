using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderApi.Data;
using OrderApi.DTOs;
using OrderApi.Models;
using SharedLibrary.Enums;

namespace OrderApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class OrdersController : ControllerBase
    {
        private readonly OrderDbContext _context;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(OrderDbContext context, ILogger<OrdersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>Get all orders</summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<OrderResponseDto>), 200)]
        public async Task<IActionResult> GetAll([FromQuery] OrderStatus? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var query = _context.Orders.Include(o => o.Items).AsQueryable();

            if (status.HasValue)
                query = query.Where(o => o.Status == status.Value);

            var total = await query.CountAsync();
            var orders = await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            Response.Headers.Append("X-Total-Count", total.ToString());
            return Ok(orders.Select(MapToDto));
        }

        /// <summary>Get order by ID</summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(OrderResponseDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var order = await _context.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id);
            if (order is null) return NotFound(new { message = $"Order {id} not found." });
            return Ok(MapToDto(order));
        }

        /// <summary>Create a new order</summary>
        [HttpPost]
        [ProducesResponseType(typeof(OrderResponseDto), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] CreateOrderDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (!dto.Items.Any()) return BadRequest(new { message = "Order must have at least one item." });

            var order = new Order
            {
                CustomerName = dto.CustomerName,
                CustomerEmail = dto.CustomerEmail,
                Currency = dto.Currency,
                Notes = dto.Notes,
                Items = dto.Items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };

            order.TotalAmount = order.Items.Sum(i => i.Quantity * i.UnitPrice);

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Order created: {OrderId} for {CustomerEmail}", order.Id, order.CustomerEmail);

            return CreatedAtAction(nameof(GetById), new { id = order.Id }, MapToDto(order));
        }

        /// <summary>Update an order</summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(OrderResponseDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateOrderDto dto)
        {
            var order = await _context.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id);
            if (order is null) return NotFound(new { message = $"Order {id} not found." });

            if (order.Status is OrderStatus.Delivered or OrderStatus.Cancelled)
                return BadRequest(new { message = $"Cannot update a {order.Status} order." });

            if (dto.CustomerName is not null) order.CustomerName = dto.CustomerName;
            if (dto.Notes is not null) order.Notes = dto.Notes;
            if (dto.Status.HasValue) order.Status = dto.Status.Value;

            order.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Order updated: {OrderId}", order.Id);
            return Ok(MapToDto(order));
        }

        /// <summary>Delete an order</summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order is null) return NotFound(new { message = $"Order {id} not found." });

            if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Cancelled)
                return BadRequest(new { message = "Only Pending or Cancelled orders can be deleted." });

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Order deleted: {OrderId}", id);
            return NoContent();
        }

        private static OrderResponseDto MapToDto(Order o) => new()
        {
            Id = o.Id,
            CustomerName = o.CustomerName,
            CustomerEmail = o.CustomerEmail,
            TotalAmount = o.TotalAmount,
            Currency = o.Currency,
            Status = o.Status,
            Notes = o.Notes,
            CreatedAt = o.CreatedAt,
            UpdatedAt = o.UpdatedAt,
            Items = o.Items.Select(i => new OrderItemResponseDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                TotalPrice = i.TotalPrice
            }).ToList()
        };
    }
}
