using Microsoft.AspNetCore.SignalR;
using SignalRChat.Models;

namespace SignalRChat.Hubs;

public partial class ChatHub
{
    public async Task SetTyping(string roomName, bool isTyping)
    {
        var name = GetCallerName();
        if (name != null)
            await Clients.OthersInGroup(roomName).SendAsync("UserTyping", roomName, name, isTyping);
    }

    public async Task<OperationResult> DeleteMessage(string roomName, string messageId)
    {
        var (ok, err) = moderation.DeleteMessage(Context.ConnectionId, roomName, messageId);
        if (!ok) return new OperationResult(false, err);

        await Clients.Group(roomName).SendAsync("MessageDeleted", roomName, messageId);
        return new OperationResult(true);
    }

    public async Task<OperationResult> KickUser(string roomName, string targetUsername) =>
        await ApplyKickOrBan(roomName, targetUsername, ban: false);

    public async Task<OperationResult> BanUser(string roomName, string targetUsername) =>
        await ApplyKickOrBan(roomName, targetUsername, ban: true);

    public async Task<OperationResult> UnbanUser(string roomName, string targetUsername)
    {
        var (ok, err, banned, sysMsg) = moderation.Unban(Context.ConnectionId, roomName, targetUsername);
        if (!ok) return new OperationResult(false, err);

        if (sysMsg != null) await Clients.Group(roomName).SendAsync("ReceiveMessage", sysMsg);
        await Clients.Group(roomName).SendAsync("BannedUsersUpdated", roomName, banned);
        return new OperationResult(true);
    }

    public async Task<OperationResult> MuteUser(string roomName, string targetUsername, int minutes)
    {
        var (ok, err, sysMsg) = moderation.Mute(Context.ConnectionId, roomName, targetUsername, minutes);
        if (!ok) return new OperationResult(false, err);

        if (sysMsg != null) await Clients.Group(roomName).SendAsync("ReceiveMessage", sysMsg);
        await Clients.Group(roomName).SendAsync("RoomUsersUpdated", roomName, rooms.GetRoomUsers(roomName));
        return new OperationResult(true);
    }

    public async Task<OperationResult> ToggleModerator(string roomName, string targetUsername)
    {
        var (ok, err, promoted, targetConn, sysMsg) = moderation.ToggleModerator(Context.ConnectionId, roomName, targetUsername);
        if (!ok) return new OperationResult(false, err);

        if (sysMsg != null) await Clients.Group(roomName).SendAsync("ReceiveMessage", sysMsg);
        await Clients.Group(roomName).SendAsync("RoomUsersUpdated", roomName, rooms.GetRoomUsers(roomName));
        if (targetConn != null)
            await Clients.Client(targetConn).SendAsync("RoleChanged", roomName, promoted ? "Moderator" : "User");
        return new OperationResult(true);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var user = store.RemoveConnection(Context.ConnectionId);
        if (user?.CurrentRoom != null)
        {
            var msg = rooms.AddSystemMessage(user.CurrentRoom, $"{user.Username} отключился(лась).");
            if (msg != null) await Clients.Group(user.CurrentRoom).SendAsync("ReceiveMessage", msg);
            await Clients.Group(user.CurrentRoom).SendAsync("RoomUsersUpdated", user.CurrentRoom, rooms.GetRoomUsers(user.CurrentRoom));
            await Clients.All.SendAsync("RoomsUpdated", rooms.GetRooms());
        }
        await base.OnDisconnectedAsync(exception);
    }

    private async Task<OperationResult> ApplyKickOrBan(string roomName, string targetUsername, bool ban)
    {
        var (ok, err, targetConn, sysMsg) = moderation.KickOrBan(Context.ConnectionId, roomName, targetUsername, ban);
        if (!ok) return new OperationResult(false, err);

        if (targetConn != null)
        {
            await Groups.RemoveFromGroupAsync(targetConn, roomName);
            await Clients.Client(targetConn).SendAsync("KickedFromRoom", roomName, ban);
        }
        if (sysMsg != null) await Clients.Group(roomName).SendAsync("ReceiveMessage", sysMsg);
        await Clients.Group(roomName).SendAsync("RoomUsersUpdated", roomName, rooms.GetRoomUsers(roomName));
        if (ban)
            await Clients.Group(roomName).SendAsync("BannedUsersUpdated", roomName, GetBanned(roomName));
        await Clients.All.SendAsync("RoomsUpdated", rooms.GetRooms());
        return new OperationResult(true);
    }

    private string? GetCallerName()
    {
        lock (store.SyncRoot)
            return store.UsersByConnection.TryGetValue(Context.ConnectionId, out var u) ? u.Username : null;
    }

    private List<string> GetBanned(string roomName)
    {
        lock (store.SyncRoot)
            return store.Rooms.TryGetValue(roomName, out var r) ? r.BannedUsers.OrderBy(x => x).ToList() : [];
    }
}
