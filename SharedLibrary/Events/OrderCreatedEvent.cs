namespace SharedLibrary.Events
{
    /// <summary>
    /// Event raised when an order is created in the Order service.
    /// Published on the event bus as "ORDER_CREATED".
    /// </summary>
    public class OrderCreatedEvent
    {
        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerEmail { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; } = "USD";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}