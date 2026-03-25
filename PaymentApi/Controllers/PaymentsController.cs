using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentApi.Data;
using PaymentApi.DTOs;
using PaymentApi.Models;
using SharedLibrary.Enums;

namespace PaymentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class PaymentsController : ControllerBase
    {
        private readonly PaymentDbContext _context;
        private readonly ILogger<PaymentsController> _logger;

        public PaymentsController(PaymentDbContext context, ILogger<PaymentsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>Get all payments</summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PaymentResponseDto>), 200)]
        public async Task<IActionResult> GetAll([FromQuery] PaymentStatus? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var query = _context.Payments.AsQueryable();

            if (status.HasValue)
                query = query.Where(p => p.Status == status.Value);

            var total = await query.CountAsync();
            var payments = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            Response.Headers.Append("X-Total-Count", total.ToString());
            return Ok(payments.Select(MapToDto));
        }

        /// <summary>Get payment by ID</summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(PaymentResponseDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment is null) return NotFound(new { message = $"Payment {id} not found." });
            return Ok(MapToDto(payment));
        }

        /// <summary>Get all payments for a specific order</summary>
        [HttpGet("order/{orderId:guid}")]
        [ProducesResponseType(typeof(IEnumerable<PaymentResponseDto>), 200)]
        public async Task<IActionResult> GetByOrder(Guid orderId)
        {
            var payments = await _context.Payments
                .Where(p => p.OrderId == orderId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
            return Ok(payments.Select(MapToDto));
        }

        /// <summary>Process a payment for an order</summary>
        [HttpPost("process")]
        [ProducesResponseType(typeof(PaymentResponseDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        public async Task<IActionResult> ProcessPayment([FromBody] ProcessPaymentDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (dto.Amount <= 0) return BadRequest(new { message = "Amount must be greater than zero." });

            // Check if a completed payment already exists for this order
            var existingCompleted = await _context.Payments
                .AnyAsync(p => p.OrderId == dto.OrderId && p.Status == PaymentStatus.Completed);
            if (existingCompleted)
                return Conflict(new { message = "A completed payment already exists for this order." });

            // Simulate payment gateway processing
            var transactionId = $"TXN-{Guid.NewGuid():N}".ToUpper()[..20];
            var isSuccess = SimulateGateway(dto.Method, dto.PaymentToken);

            var payment = new Payment
            {
                OrderId = dto.OrderId,
                Amount = dto.Amount,
                Currency = dto.Currency,
                Method = dto.Method,
                Status = isSuccess ? PaymentStatus.Completed : PaymentStatus.Failed,
                TransactionId = isSuccess ? transactionId : null,
                FailureReason = isSuccess ? null : "Gateway declined the transaction."
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Payment {PaymentId} for Order {OrderId}: {Status}", payment.Id, dto.OrderId, payment.Status);

            var response = MapToDto(payment);
            if (!isSuccess)
                return UnprocessableEntity(response); // 422 — payment created but failed

            return CreatedAtAction(nameof(GetById), new { id = payment.Id }, response);
        }

        /// <summary>Refund a completed payment</summary>
        [HttpPut("{id:guid}/refund")]
        [ProducesResponseType(typeof(PaymentResponseDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Refund(Guid id, [FromBody] RefundPaymentDto dto)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment is null) return NotFound(new { message = $"Payment {id} not found." });

            if (payment.Status != PaymentStatus.Completed)
                return BadRequest(new { message = $"Only Completed payments can be refunded. Current status: {payment.Status}." });

            payment.Status = PaymentStatus.Refunded;
            payment.RefundTransactionId = $"REF-{Guid.NewGuid():N}".ToUpper()[..20];
            payment.RefundedAt = DateTime.UtcNow;
            payment.UpdatedAt = DateTime.UtcNow;
            payment.FailureReason = dto.Reason;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Payment {PaymentId} refunded. RefundTxn: {RefundTxn}", id, payment.RefundTransactionId);
            return Ok(MapToDto(payment));
        }

        /// <summary>Simulate payment gateway — replace with real gateway SDK in production</summary>
        private static bool SimulateGateway(PaymentMethod method, string? token)
        {
            // CashOnDelivery always succeeds; others require a token
            if (method == PaymentMethod.CashOnDelivery) return true;
            return !string.IsNullOrWhiteSpace(token);
        }

        private static PaymentResponseDto MapToDto(Payment p) => new()
        {
            Id = p.Id,
            OrderId = p.OrderId,
            Amount = p.Amount,
            Currency = p.Currency,
            Status = p.Status,
            Method = p.Method.ToString(),
            TransactionId = p.TransactionId,
            FailureReason = p.FailureReason,
            RefundTransactionId = p.RefundTransactionId,
            RefundedAt = p.RefundedAt,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        };
    }
}
