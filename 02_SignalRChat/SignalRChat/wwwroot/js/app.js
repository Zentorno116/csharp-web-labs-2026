"use strict";

(async function bootstrap() {
  const app = window.ChatApp;
  const conn = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .withAutomaticReconnect()
    .build();
  app.connection = conn;

  conn.on("RoomsUpdated", (rooms) => { app.rooms = rooms; app.renderRooms(); });
  conn.on("ReceiveMessage", (msg) => {
    if (app.activeRoom && msg.roomName.toLowerCase() === app.activeRoom.name.toLowerCase()) {
      app.appendMessage(msg);
    }
  });
  conn.on("RoomUsersUpdated", (roomName, users) => {
    if (app.activeRoom?.name.toLowerCase() === roomName.toLowerCase()) {
      app.users = users;
      const self = users.find((u) => u.username.toLowerCase() === app.me?.username.toLowerCase());
      if (self) app.myRole = self.role;
      app.renderHeader();
      app.renderMembers();
    }
  });
  conn.on("BannedUsersUpdated", (roomName, banned) => {
    if (app.activeRoom?.name.toLowerCase() === roomName.toLowerCase()) {
      app.bannedUsers = banned;
      app.renderHeader();
    }
  });
  conn.on("MessageDeleted", (roomName, msgId) => {
    if (app.activeRoom?.name.toLowerCase() !== roomName.toLowerCase()) return;
    const row = document.querySelector(`.msg-row[data-msg-id="${msgId}"]`);
    if (row) {
      const body = row.querySelector(".msg-text");
      body.textContent = "Сообщение удалено модератором";
      body.classList.add("msg-deleted");
      row.querySelector(".msg-del-btn")?.remove();
    }
  });
  conn.on("UserTyping", (roomName, user, isTyping) => {
    if (app.activeRoom?.name.toLowerCase() !== roomName.toLowerCase()) return;
    if (isTyping) app.typingUsers.add(user); else app.typingUsers.delete(user);
    app.renderTyping();
  });
  conn.on("RoleChanged", (roomName, role) => {
    if (app.activeRoom?.name.toLowerCase() === roomName.toLowerCase()) {
      app.myRole = role;
      app.renderHeader();
      app.renderMembers();
    }
    app.toast(`Ваша роль в #${roomName}: ${app.roleLabel(role)}`);
  });
  conn.on("KickedFromRoom", (roomName, isBan) => {
    app.toast(isBan ? `Вы забанены в #${roomName}` : `Вас кикнули из #${roomName}`, true);
    app.activeRoom = null;
    app.users = [];
    app.el("messagesFeed").innerHTML = "";
    app.renderHeader();
    app.renderMembers();
    app.renderRooms();
  });
  conn.on("RoomDeleted", (roomName) => {
    app.toast(`Комната #${roomName} была удалена владельцем.`);
    if (app.activeRoom?.name.toLowerCase() === roomName.toLowerCase()) app.joinRoom("Общий");
  });

  app.el("authForm").onsubmit = async (e) => {
    e.preventDefault();
    await app.login(app.el("usernameInput").value);
  };
  app.el("messageForm").onsubmit = async (e) => {
    e.preventDefault();
    const input = app.el("messageInput");
    if (!input.value.trim()) return;
    await app.sendMessage(input.value);
    input.value = "";
    conn.invoke("SetTyping", app.activeRoom.name, false).catch(() => {});
  };
  app.el("messageInput").addEventListener("input", () => {
    if (!app.activeRoom) return;
    conn.invoke("SetTyping", app.activeRoom.name, true).catch(() => {});
    clearTimeout(app.typingTimer);
    app.typingTimer = setTimeout(() => conn.invoke("SetTyping", app.activeRoom.name, false).catch(() => {}), 1800);
  });
  app.el("openCreateRoomBtn").onclick = () => app.openCreateRoomModal();
  app.el("openBansBtn").onclick = () => app.openBansModal();
  app.el("deleteRoomBtn").onclick = async () => {
    if (app.activeRoom && confirm(`Удалить комнату #${app.activeRoom.name}?`)) {
      const res = await conn.invoke("DeleteRoom", app.activeRoom.name);
      if (!res.success) app.toast(res.error, true);
    }
  };
  app.el("themeToggleBtn").onclick = () => {
    const html = document.documentElement;
    html.dataset.theme = html.dataset.theme === "dark" ? "light" : "dark";
  };

  await conn.start();
})();
