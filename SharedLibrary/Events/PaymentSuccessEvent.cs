namespace SharedLibrary.Events
{
    /// <summary>
    /// Event published when payment succeeds — "PAYMENT_SUCCESS".
    /// Consumed by Order and Inventory services.
    /// </summary>
    public class PaymentSuccessEvent
    {
        public Guid PaymentId { get; set; }
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
    }
}