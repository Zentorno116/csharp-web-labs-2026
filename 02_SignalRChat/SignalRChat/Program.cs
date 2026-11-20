using SignalRChat.Hubs;
using SignalRChat.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddSingleton<ChatStore>();
builder.Services.AddSingleton<ChatRoomService>();
builder.Services.AddSingleton<ChatModerationService>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapHub<ChatHub>("/chatHub");

app.Run();
