using SignalRChat.Models;

namespace SignalRChat.Services;

public class ChatRoomService(ChatStore store)
{
    public List<RoomSummaryDto> GetRooms()
    {
        lock (store.SyncRoot)
        {
            return store.Rooms.Values
                .OrderByDescending(r => r.IsDefault)
                .ThenBy(r => r.Name, StringComparer.OrdinalIgnoreCase)
                .Select(r => new RoomSummaryDto(
                    r.Name,
                    r.Description,
                    r.OwnerUsername,
                    r.IsDefault,
                    store.UsersByConnection.Values.Count(u =>
                        string.Equals(u.CurrentRoom, r.Name, StringComparison.OrdinalIgnoreCase))))
                .ToList();
        }
    }

    public OperationResult CreateRoom(string connectionId, string rawName, string rawDesc)
    {
        var name = rawName.Trim();
        if (name.Length is < 2 or > 30)
            return new OperationResult(false, "Название комнаты: от 2 до 30 символов.");

        lock (store.SyncRoot)
        {
            if (!store.UsersByConnection.TryGetValue(connectionId, out var user))
                return new OperationResult(false, "Сначала представьтесь в чате.");
            if (store.Rooms.ContainsKey(name))
                return new OperationResult(false, "Комната с таким названием уже существует.");

            var room = new ChatRoom
            {
                Name = name,
                Description = string.IsNullOrWhiteSpace(rawDesc) ? "Комната пользователей" : rawDesc.Trim(),
                OwnerUsername = user.Username
            };
            room.Messages.Add(new ChatMessage
            {
                RoomName = name,
                Sender = "Система",
                Text = $"{user.Username} создал(а) комнату «{name}».",
                IsSystem = true
            });
            store.Rooms[name] = room;
            return new OperationResult(true);
        }
    }

    public (bool Success, string? Error, List<string> AffectedConnections) DeleteRoom(string connectionId, string roomName)
    {
        lock (store.SyncRoot)
        {
            if (!store.UsersByConnection.TryGetValue(connectionId, out var user))
                return (false, "Пользователь не найден.", []);
            if (!store.Rooms.TryGetValue(roomName, out var room))
                return (false, "Комната не найдена.", []);
            if (room.IsDefault)
                return (false, "Нельзя удалить базовую комнату «Общий».", []);
            if (store.GetRole(room, user.Username) != UserRole.Owner)
                return (false, "Удалить комнату может только её владелец.", []);

            var affected = store.UsersByConnection.Values
                .Where(u => string.Equals(u.CurrentRoom, room.Name, StringComparison.OrdinalIgnoreCase))
                .Select(u => { u.CurrentRoom = null; return u.ConnectionId; })
                .ToList();

            store.Rooms.Remove(room.Name);
            return (true, null, affected);
        }
    }

    public (bool Success, string? Error, string? PreviousRoom, RoomSnapshotDto? Snapshot, MessageDto? JoinMsg)
        JoinRoom(string connectionId, string roomName)
    {
        lock (store.SyncRoot)
        {
            if (!store.UsersByConnection.TryGetValue(connectionId, out var user))
                return (false, "Сначала введите имя.", null, null, null);
            if (!store.Rooms.TryGetValue(roomName, out var room))
                return (false, "Комната не найдена.", null, null, null);
            if (room.BannedUsers.Contains(user.Username))
                return (false, $"Вы забанены в комнате «{room.Name}».", null, null, null);

            var previous = user.CurrentRoom;
            user.CurrentRoom = room.Name;

            var sysMsg = AddSystemMessageUnsafe(room, $"{user.Username} присоединился(лась) к комнате.");
            return (true, null, previous, BuildSnapshotUnsafe(room, user.Username), ToDto(sysMsg));
        }
    }

    public (bool Success, string? Error, MessageDto? Message) SendMessage(string connectionId, string roomName, string rawText)
    {
        var text = rawText.Trim();
        if (string.IsNullOrEmpty(text) || text.Length > 1000)
            return (false, "Сообщение должно быть от 1 до 1000 символов.", null);

        lock (store.SyncRoot)
        {
            if (!store.UsersByConnection.TryGetValue(connectionId, out var user))
                return (false, "Пользователь не авторизован.", null);
            if (!store.Rooms.TryGetValue(roomName, out var room))
                return (false, "Комната не найдена.", null);
            if (!string.Equals(user.CurrentRoom, room.Name, StringComparison.OrdinalIgnoreCase))
                return (false, "Вы не находитесь в этой комнате.", null);
            if (store.IsMuted(room, user.Username, out var until))
                return (false, $"Вы в муте до {until!.Value.ToLocalTime():HH:mm:ss}.", null);

            var msg = new ChatMessage
            {
                RoomName = room.Name,
                Sender = user.Username,
                SenderRole = store.GetRole(room, user.Username),
                AvatarColor = user.AvatarColor,
                Text = text
            };
            room.Messages.Add(msg);
            if (room.Messages.Count > 200) room.Messages.RemoveAt(0);
            return (true, null, ToDto(msg));
        }
    }

    public List<RoomUserDto> GetRoomUsers(string roomName)
    {
        lock (store.SyncRoot)
        {
            return store.Rooms.TryGetValue(roomName, out var room) ? BuildUsersUnsafe(room) : [];
        }
    }

    public MessageDto? AddSystemMessage(string roomName, string text)
    {
        lock (store.SyncRoot)
        {
            return store.Rooms.TryGetValue(roomName, out var room)
                ? ToDto(AddSystemMessageUnsafe(room, text))
                : null;
        }
    }

    internal ChatMessage AddSystemMessageUnsafe(ChatRoom room, string text)
    {
        var msg = new ChatMessage { RoomName = room.Name, Sender = "Система", Text = text, IsSystem = true };
        room.Messages.Add(msg);
        if (room.Messages.Count > 200) room.Messages.RemoveAt(0);
        return msg;
    }

    internal RoomSnapshotDto BuildSnapshotUnsafe(ChatRoom room, string viewerUsername) => new(
        room.Name,
        room.Description,
        room.OwnerUsername,
        room.IsDefault,
        store.GetRole(room, viewerUsername).ToString(),
        BuildUsersUnsafe(room),
        room.Messages.TakeLast(100).Select(ToDto).ToList(),
        room.BannedUsers.OrderBy(x => x, StringComparer.OrdinalIgnoreCase).ToList());

    internal List<RoomUserDto> BuildUsersUnsafe(ChatRoom room) =>
        store.UsersByConnection.Values
            .Where(u => string.Equals(u.CurrentRoom, room.Name, StringComparison.OrdinalIgnoreCase))
            .Select(u =>
            {
                var muted = store.IsMuted(room, u.Username, out var until);
                return new RoomUserDto(u.Username, u.AvatarColor, store.GetRole(room, u.Username).ToString(), muted, until);
            })
            .OrderByDescending(u => u.Role == nameof(UserRole.Owner))
            .ThenByDescending(u => u.Role == nameof(UserRole.Moderator))
            .ThenBy(u => u.Username, StringComparer.OrdinalIgnoreCase)
            .ToList();

    internal static MessageDto ToDto(ChatMessage m) => new(
        m.Id, m.RoomName, m.Sender, m.SenderRole.ToString(), m.AvatarColor,
        m.IsDeleted ? "Сообщение удалено модератором" : m.Text,
        m.TimestampUtc, m.IsSystem, m.IsDeleted);
}
