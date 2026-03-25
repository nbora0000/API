namespace SharedLibrary.Events
{
    /// <summary>
    /// Event raised when an order is placed — consumed by PaymentApi.
    /// </summary>
    public class OrderPlacedEvent
    {
        public Guid OrderId { get; set; }
        public string CustomerEmail { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; } = "USD";
        public DateTime PlacedAt { get; set; } = DateTime.UtcNow;
    }
}
