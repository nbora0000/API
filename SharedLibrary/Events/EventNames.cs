namespace SharedLibrary.Events
{
    /// <summary>
    /// Centralized event name constants for the event bus.
    /// </summary>
    public static class EventNames
    {
        public const string OrderCreated = "ORDER_CREATED";
        public const string PaymentSuccess = "PAYMENT_SUCCESS";
        public const string InventoryReserved = "INVENTORY_RESERVED";
    }
}