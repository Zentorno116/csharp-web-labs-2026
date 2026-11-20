namespace SignalRChat.Models;

public enum UserRole
{
    User = 0,
    Moderator = 1,
    Owner = 2
}

public class ChatUser
{
    public required string ConnectionId { get; set; }
    public required string Username { get; set; }
    public required string AvatarColor { get; set; }
    public string? CurrentRoom { get; set; }
}

public class ChatMessage
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..10];
    public required string RoomName { get; set; }
    public required string Sender { get; set; }
    public UserRole SenderRole { get; set; }
    public string AvatarColor { get; set; } = "#6366f1";
    public required string Text { get; set; }
    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
    public bool IsSystem { get; set; }
    public bool IsDeleted { get; set; }
}

public class ChatRoom
{
    public required string Name { get; set; }
    public string Description { get; set; } = string.Empty;
    public required string OwnerUsername { get; set; }
    public bool IsDefault { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public HashSet<string> Moderators { get; } = new(StringComparer.OrdinalIgnoreCase);
    public HashSet<string> BannedUsers { get; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, DateTime> MutedUsers { get; } = new(StringComparer.OrdinalIgnoreCase);
    public List<ChatMessage> Messages { get; } = [];
}
