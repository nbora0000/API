namespace SharedLibrary.Events
{
    /// <summary>
    /// Event published when inventory is reserved for an order — "INVENTORY_RESERVED".
    /// Consumed by Order service to continue processing.
    /// </summary>
    public class InventoryReservedEvent
    {
        public Guid ReservationId { get; set; }
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime ReservedAt { get; set; } = DateTime.UtcNow;
    }
}