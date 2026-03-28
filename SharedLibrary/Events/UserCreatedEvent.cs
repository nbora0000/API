namespace SharedLibrary.Events
{
    /// <summary>
    /// Event emitted when a user is created — published as "USER_CREATED".
    /// </summary>
    public class UserCreatedEvent
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}