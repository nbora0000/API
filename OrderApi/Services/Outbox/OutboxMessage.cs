using System.ComponentModel.DataAnnotations;

namespace OrderApi.Services.Outbox
{
    public class OutboxMessage
    {
        [Key]
        public Guid Id { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string Payload { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool Dispatched { get; set; }
    }
}