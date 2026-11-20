using SignalRChat.Models;

namespace SignalRChat.Services;

public class ChatStore
{
    public object SyncRoot { get; } = new();
    public Dictionary<string, ChatUser> UsersByConnection { get; } = new();
    public Dictionary<string, ChatRoom> Rooms { get; } = new(StringComparer.OrdinalIgnoreCase);

    private static readonly string[] Palette =
        ["#6366f1", "#ec4899", "#14b8a6", "#f59e0b", "#8b5cf6", "#06b6d4", "#ef4444", "#22c55e"];

    public ChatStore()
    {
        SeedRoom("Общий", "Главная комната для всех участников", "admin", isDefault: true);
        SeedRoom("Учёба КФУ", "Обсуждение лабораторных, LINQ, SignalR и ASP.NET Core", "admin");
        SeedRoom("Разработка", "Код, архитектура, PostgreSQL и фронтенд", "admin");
    }

    private void SeedRoom(string name, string description, string owner, bool isDefault = false)
    {
        var room = new ChatRoom { Name = name, Description = description, OwnerUsername = owner, IsDefault = isDefault };
        room.Moderators.Add("moderator");
        room.Messages.Add(new ChatMessage
        {
            RoomName = name,
            Sender = "Система",
            Text = $"Добро пожаловать в комнату «{name}»! {description}.",
            IsSystem = true
        });
        Rooms[name] = room;
    }

    public UserRole GetRole(ChatRoom room, string username)
    {
        if (room.OwnerUsername.Equals(username, StringComparison.OrdinalIgnoreCase))
            return UserRole.Owner;
        if (room.Moderators.Contains(username))
            return UserRole.Moderator;
        return UserRole.User;
    }

    public bool IsMuted(ChatRoom room, string username, out DateTime? untilUtc)
    {
        if (room.MutedUsers.TryGetValue(username, out var exp))
        {
            if (exp > DateTime.UtcNow)
            {
                untilUtc = exp;
                return true;
            }
            room.MutedUsers.Remove(username);
        }
        untilUtc = null;
        return false;
    }

    public (bool Success, string? Error, ChatUser? User) RegisterUser(string connectionId, string rawUsername)
    {
        var username = rawUsername.Trim();
        if (username.Length is < 2 or > 24)
            return (false, "Имя должно быть от 2 до 24 символов.", null);

        lock (SyncRoot)
        {
            var taken = UsersByConnection.Values.Any(u =>
                u.ConnectionId != connectionId &&
                u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            if (taken)
                return (false, "Пользователь с таким именем уже онлайн.", null);

            var color = Palette[Math.Abs(username.GetHashCode()) % Palette.Length];
            var user = new ChatUser { ConnectionId = connectionId, Username = username, AvatarColor = color };
            UsersByConnection[connectionId] = user;
            return (true, null, user);
        }
    }

    public ChatUser? RemoveConnection(string connectionId)
    {
        lock (SyncRoot)
        {
            UsersByConnection.Remove(connectionId, out var user);
            return user;
        }
    }
}
