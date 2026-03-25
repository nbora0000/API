using PaymentApi.Models;
using SharedLibrary.Enums;

namespace PaymentApi.DTOs
{
    public class ProcessPaymentDto
    {
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public PaymentMethod Method { get; set; }
        /// <summary>Card/wallet token from payment gateway SDK</summary>
        public string? PaymentToken { get; set; }
    }

    public class RefundPaymentDto
    {
        public string? Reason { get; set; }
    }

    public class PaymentResponseDto
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public PaymentStatus Status { get; set; }
        public string Method { get; set; } = string.Empty;
        public string? TransactionId { get; set; }
        public string? FailureReason { get; set; }
        public string? RefundTransactionId { get; set; }
        public DateTime? RefundedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
