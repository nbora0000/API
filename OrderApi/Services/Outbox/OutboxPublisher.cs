using Microsoft.EntityFrameworkCore;
using SharedLibrary.Events;
using System.Text.Json;

namespace OrderApi.Services.Outbox
{
    public class OutboxPublisher
    {
        private readonly OutboxDbContext _db;
        private readonly ILogger<OutboxPublisher> _logger;

        public OutboxPublisher(OutboxDbContext db, ILogger<OutboxPublisher> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task EnqueueAsync(string eventName, object payload)
        {
            var msg = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                EventName = eventName,
                Payload = JsonSerializer.Serialize(payload),
                CreatedAt = DateTime.UtcNow,
                Dispatched = false
            };
            _db.OutboxMessages.Add(msg);
            await _db.SaveChangesAsync();
        }

        // Naive dispatcher for demo — in real app this would publish to the event bus and mark dispatched
        public async Task DispatchPendingAsync()
        {
            var pending = await _db.OutboxMessages.Where(m => !m.Dispatched).ToListAsync();
            foreach (var p in pending)
            {
                _logger.LogInformation("Dispatching event {EventName}: {Payload}", p.EventName, p.Payload);
                p.Dispatched = true;
            }
            await _db.SaveChangesAsync();
        }
    }
}