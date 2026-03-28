using Microsoft.EntityFrameworkCore;

namespace OrderApi.Services.Outbox
{
    public class OutboxDbContext : DbContext
    {
        public OutboxDbContext(DbContextOptions<OutboxDbContext> options) : base(options) { }
        public DbSet<OutboxMessage> OutboxMessages { get; set; }
    }
}