using SignalRChat.Services;
using Xunit;

namespace SignalRChat.Tests;

public class ChatModerationTests
{
    [Fact]
    public void OwnerCanPromoteModeratorMuteBanAndDeleteMessages()
    {
        var store = new ChatStore();
        var rooms = new ChatRoomService(store);
        var mod = new ChatModerationService(store, rooms);

        Assert.True(store.RegisterUser("c1", "alice").Success);
        Assert.True(store.RegisterUser("c2", "bob").Success);
        Assert.True(store.RegisterUser("c3", "charlie").Success);

        Assert.True(rooms.CreateRoom("c1", "DevRoom", "Test").Success);
        Assert.True(rooms.JoinRoom("c1", "DevRoom").Success);
        Assert.True(rooms.JoinRoom("c2", "DevRoom").Success);
        Assert.True(rooms.JoinRoom("c3", "DevRoom").Success);

        // Regular user cannot mute
        Assert.False(mod.Mute("c2", "DevRoom", "charlie", 5).Success);

        // Owner promotes bob to moderator
        var promo = mod.ToggleModerator("c1", "DevRoom", "bob");
        Assert.True(promo.Success);
        Assert.True(promo.IsNowModerator);

        // Moderator bob can mute charlie
        Assert.True(mod.Mute("c2", "DevRoom", "charlie", 5).Success);
        Assert.False(rooms.SendMessage("c3", "DevRoom", "Hello while muted").Success);

        // Unmute allows sending again; moderator can delete charlie's message
        Assert.True(mod.Mute("c2", "DevRoom", "charlie", 0).Success);
        var sent = rooms.SendMessage("c3", "DevRoom", "Allowed message");
        Assert.True(sent.Success);
        Assert.True(mod.DeleteMessage("c2", "DevRoom", sent.Message!.Id).Success);

        // Moderator cannot ban owner
        Assert.False(mod.KickOrBan("c2", "DevRoom", "alice", ban: true).Success);

        // Moderator can ban charlie; charlie cannot rejoin until unbanned
        Assert.True(mod.KickOrBan("c2", "DevRoom", "charlie", ban: true).Success);
        Assert.False(rooms.JoinRoom("c3", "DevRoom").Success);

        Assert.True(mod.Unban("c2", "DevRoom", "charlie").Success);
        Assert.True(rooms.JoinRoom("c3", "DevRoom").Success);
    }
}
