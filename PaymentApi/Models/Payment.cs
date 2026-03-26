using SharedLibrary.Models;
using SharedLibrary.Enums;

namespace PaymentApi.Models
{
    public class Payment : AuditableEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public PaymentMethod Method { get; set; }
        public string? TransactionId { get; set; }
        public string? FailureReason { get; set; }
        public string? RefundTransactionId { get; set; }
        public DateTime? RefundedAt { get; set; }
    }

    public enum PaymentMethod
    {
        CreditCard = 0,
        DebitCard = 1,
        BankTransfer = 2,
        DigitalWallet = 3,
        CashOnDelivery = 4
    }
}
