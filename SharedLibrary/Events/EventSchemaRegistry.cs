using System.Collections.Concurrent;

namespace SharedLibrary.Events
{
    /// <summary>
    /// Simple in-memory registry that maps topics and event names to JSON schema file locations and versions.
    /// Consumers/producers can use this to discover schema locations and enforce validation.
    /// </summary>
    public static class EventSchemaRegistry
    {
        // Topic names
        public const string OrdersTopic = "orders-topic";
        public const string PaymentsTopic = "payments-topic";
        public const string InventoryTopic = "inventory-topic";
        public const string UsersTopic = "users-topic";
        public const string NotificationsTopic = "notifications-topic";

        // Map: event name -> version -> schema resource path (relative to repository)
        private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, string>> _schemas
            = new();

        static EventSchemaRegistry()
        {
            // OrderCreated
            Register("ORDER_CREATED", "v1", "SharedLibrary/Events/Schemas/v1/order-created.schema.json");
            Register("ORDER_CREATED", "v2", "SharedLibrary/Events/Schemas/v2/order-created.schema.json");

            // PaymentSuccess
            Register("PAYMENT_SUCCESS", "v1", "SharedLibrary/Events/Schemas/v1/payment-success.schema.json");
            Register("PAYMENT_SUCCESS", "v2", "SharedLibrary/Events/Schemas/v2/payment-success.schema.json");

            // InventoryReserved
            Register("INVENTORY_RESERVED", "v1", "SharedLibrary/Events/Schemas/v1/inventory-reserved.schema.json");
            Register("INVENTORY_RESERVED", "v2", "SharedLibrary/Events/Schemas/v2/inventory-reserved.schema.json");

            // UserCreated
            Register("USER_CREATED", "v1", "SharedLibrary/Events/Schemas/v1/user-created.schema.json");
            Register("USER_CREATED", "v2", "SharedLibrary/Events/Schemas/v2/user-created.schema.json");
        }

        public static void Register(string eventName, string version, string schemaPath)
        {
            var versions = _schemas.GetOrAdd(eventName, _ => new ConcurrentDictionary<string, string>());
            versions[version] = schemaPath;
        }

        public static bool TryGetSchemaPath(string eventName, string version, out string? schemaPath)
        {
            schemaPath = null;
            if (!_schemas.TryGetValue(eventName, out var versions)) return false;
            return versions.TryGetValue(version, out schemaPath);
        }

        public static IEnumerable<(string EventName, string Version, string Path)> ListAll()
        {
            foreach (var kv in _schemas)
            {
                foreach (var v in kv.Value)
                {
                    yield return (kv.Key, v.Key, v.Value);
                }
            }
        }
    }
}
