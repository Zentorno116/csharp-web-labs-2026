using Microsoft.AspNetCore.SignalR;
using SignalRChat.Models;
using SignalRChat.Services;

namespace SignalRChat.Hubs;

public partial class ChatHub(
    ChatStore store,
    ChatRoomService rooms,
    ChatModerationService moderation) : Hub
{
    public async Task<object> RegisterUser(string username)
    {
        var (ok, err, user) = store.RegisterUser(Context.ConnectionId, username);
        if (!ok || user == null)
            return new { success = false, error = err };

        return new
        {
            success = true,
            user = new { user.Username, user.AvatarColor },
            rooms = rooms.GetRooms()
        };
    }

    public async Task<OperationResult> CreateRoom(string roomName, string description)
    {
        var res = rooms.CreateRoom(Context.ConnectionId, roomName, description);
        if (res.Success)
            await Clients.All.SendAsync("RoomsUpdated", rooms.GetRooms());
        return res;
    }

    public async Task<OperationResult> DeleteRoom(string roomName)
    {
        var (ok, err, affected) = rooms.DeleteRoom(Context.ConnectionId, roomName);
        if (!ok) return new OperationResult(false, err);

        foreach (var connId in affected)
        {
            await Groups.RemoveFromGroupAsync(connId, roomName);
            await Clients.Client(connId).SendAsync("RoomDeleted", roomName);
        }
        await Clients.All.SendAsync("RoomsUpdated", rooms.GetRooms());
        return new OperationResult(true);
    }

    public async Task<object> JoinRoom(string roomName)
    {
        var (ok, err, prev, snapshot, joinMsg) = rooms.JoinRoom(Context.ConnectionId, roomName);
        if (!ok || snapshot == null)
            return new { success = false, error = err };

        if (!string.IsNullOrEmpty(prev) && !prev.Equals(roomName, StringComparison.OrdinalIgnoreCase))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, prev);
            var leaveMsg = rooms.AddSystemMessage(prev, $"{snapshot.Users.FirstOrDefault(u => u.Username == GetCallerName())?.Username ?? "Участник"} покинул(а) комнату.");
            if (leaveMsg != null) await Clients.Group(prev).SendAsync("ReceiveMessage", leaveMsg);
            await Clients.Group(prev).SendAsync("RoomUsersUpdated", prev, rooms.GetRoomUsers(prev));
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, snapshot.Name);
        if (joinMsg != null)
            await Clients.OthersInGroup(snapshot.Name).SendAsync("ReceiveMessage", joinMsg);
        await Clients.Group(snapshot.Name).SendAsync("RoomUsersUpdated", snapshot.Name, snapshot.Users);
        await Clients.All.SendAsync("RoomsUpdated", rooms.GetRooms());

        return new { success = true, snapshot };
    }

    public async Task<OperationResult> SendMessage(string roomName, string text)
    {
        var (ok, err, msg) = rooms.SendMessage(Context.ConnectionId, roomName, text);
        if (!ok || msg == null) return new OperationResult(false, err);

        await Clients.Group(msg.RoomName).SendAsync("ReceiveMessage", msg);
        return new OperationResult(true);
    }
}
