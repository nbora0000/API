namespace NotificationApi.Services
{
    public interface INotificationService
    {
        Task SendAsync(string recipient, string message, string channel = "email");
    }
}
