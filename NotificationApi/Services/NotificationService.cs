namespace NotificationApi.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> _logger;
        public NotificationService(ILogger<NotificationService> logger) { _logger = logger; }

        public Task SendAsync(string recipient, string message, string channel = "email")
        {
            _logger.LogInformation("Sending notification to {Recipient} via {Channel}: {Message}", recipient, channel, message);
            return Task.CompletedTask;
        }
    }
}
