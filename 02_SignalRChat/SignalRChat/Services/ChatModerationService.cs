using SignalRChat.Models;

namespace SignalRChat.Services;

public class ChatModerationService(ChatStore store, ChatRoomService roomService)
{
    public (bool Success, string? Error, string? TargetConnectionId, MessageDto? SysMsg)
        KickOrBan(string actorConnId, string roomName, string targetUsername, bool ban)
    {
        lock (store.SyncRoot)
        {
            var check = ValidateModerationTargetUnsafe(actorConnId, roomName, targetUsername, out var actor, out var room);
            if (!check.Success) return (false, check.Error, null, null);

            if (ban) room!.BannedUsers.Add(targetUsername);

            var target = store.UsersByConnection.Values.FirstOrDefault(u =>
                u.Username.Equals(targetUsername, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(u.CurrentRoom, room!.Name, StringComparison.OrdinalIgnoreCase));

            if (target != null) target.CurrentRoom = null;

            var action = ban ? "забанил(а)" : "кикнул(а)";
            var sysMsg = roomService.AddSystemMessageUnsafe(room!, $"{actor!.Username} {action} {targetUsername}.");
            return (true, null, target?.ConnectionId, ChatRoomService.ToDto(sysMsg));
        }
    }

    public (bool Success, string? Error, List<string> BannedUsers, MessageDto? SysMsg)
        Unban(string actorConnId, string roomName, string targetUsername)
    {
        lock (store.SyncRoot)
        {
            if (!store.UsersByConnection.TryGetValue(actorConnId, out var actor) ||
                !store.Rooms.TryGetValue(roomName, out var room))
                return (false, "Комната или пользователь не найдены.", [], null);

            if (store.GetRole(room, actor.Username) < UserRole.Moderator)
                return (false, "Недостаточно прав для разбана.", [], null);

            room.BannedUsers.Remove(targetUsername);
            var sysMsg = roomService.AddSystemMessageUnsafe(room, $"{actor.Username} разбанил(а) {targetUsername}.");
            return (true, null, room.BannedUsers.OrderBy(x => x).ToList(), ChatRoomService.ToDto(sysMsg));
        }
    }

    public (bool Success, string? Error, MessageDto? SysMsg)
        Mute(string actorConnId, string roomName, string targetUsername, int minutes)
    {
        lock (store.SyncRoot)
        {
            var check = ValidateModerationTargetUnsafe(actorConnId, roomName, targetUsername, out var actor, out var room);
            if (!check.Success) return (false, check.Error, null);

            if (minutes <= 0)
            {
                room!.MutedUsers.Remove(targetUsername);
                var msg = roomService.AddSystemMessageUnsafe(room, $"{actor!.Username} снял(а) мут с {targetUsername}.");
                return (true, null, ChatRoomService.ToDto(msg));
            }

            var mins = Math.Clamp(minutes, 1, 60);
            room!.MutedUsers[targetUsername] = DateTime.UtcNow.AddMinutes(mins);
            var sysMsg = roomService.AddSystemMessageUnsafe(room, $"{actor!.Username} замутил(а) {targetUsername} на {mins} мин.");
            return (true, null, ChatRoomService.ToDto(sysMsg));
        }
    }

    public (bool Success, string? Error, bool IsNowModerator, string? TargetConnId, MessageDto? SysMsg)
        ToggleModerator(string actorConnId, string roomName, string targetUsername)
    {
        lock (store.SyncRoot)
        {
            if (!store.UsersByConnection.TryGetValue(actorConnId, out var actor) ||
                !store.Rooms.TryGetValue(roomName, out var room))
                return (false, "Комната или пользователь не найдены.", false, null, null);

            if (store.GetRole(room, actor.Username) != UserRole.Owner)
                return (false, "Назначать модераторов может только владелец комнаты.", false, null, null);
            if (actor.Username.Equals(targetUsername, StringComparison.OrdinalIgnoreCase))
                return (false, "Владелец уже имеет максимальные права.", false, null, null);

            bool promoted;
            if (room.Moderators.Contains(targetUsername))
            {
                room.Moderators.Remove(targetUsername);
                promoted = false;
            }
            else
            {
                room.Moderators.Add(targetUsername);
                promoted = true;
            }

            var targetConn = store.UsersByConnection.Values
                .FirstOrDefault(u => u.Username.Equals(targetUsername, StringComparison.OrdinalIgnoreCase))?.ConnectionId;
            var verb = promoted ? "назначил(а) модератором" : "снял(а) права модератора с";
            var sysMsg = roomService.AddSystemMessageUnsafe(room, $"{actor.Username} {verb} {targetUsername}.");
            return (true, null, promoted, targetConn, ChatRoomService.ToDto(sysMsg));
        }
    }

    public (bool Success, string? Error) DeleteMessage(string actorConnId, string roomName, string messageId)
    {
        lock (store.SyncRoot)
        {
            if (!store.UsersByConnection.TryGetValue(actorConnId, out var actor) ||
                !store.Rooms.TryGetValue(roomName, out var room))
                return (false, "Комната или пользователь не найдены.");

            var msg = room.Messages.FirstOrDefault(m => m.Id == messageId);
            if (msg == null || msg.IsSystem)
                return (false, "Сообщение не найдено.");

            var actorRole = store.GetRole(room, actor.Username);
            var authorRole = store.GetRole(room, msg.Sender);
            var isOwn = msg.Sender.Equals(actor.Username, StringComparison.OrdinalIgnoreCase);

            if (!isOwn && !(actorRole >= UserRole.Moderator && actorRole > authorRole))
                return (false, "Недостаточно прав для удаления этого сообщения.");

            msg.IsDeleted = true;
            return (true, null);
        }
    }

    private OperationResult ValidateModerationTargetUnsafe(
        string actorConnId, string roomName, string targetUsername, out ChatUser? actor, out ChatRoom? room)
    {
        actor = null;
        room = null;
        if (!store.UsersByConnection.TryGetValue(actorConnId, out actor) ||
            !store.Rooms.TryGetValue(roomName, out room))
            return new OperationResult(false, "Комната или пользователь не найдены.");

        if (actor.Username.Equals(targetUsername, StringComparison.OrdinalIgnoreCase))
            return new OperationResult(false, "Нельзя применить это действие к себе.");

        var actorRole = store.GetRole(room, actor.Username);
        var targetRole = store.GetRole(room, targetUsername);
        if (actorRole < UserRole.Moderator || actorRole <= targetRole)
            return new OperationResult(false, "Недостаточно прав для модерации этого пользователя.");

        return new OperationResult(true);
    }
}
