"use strict";

Object.assign(window.ChatApp, {
  async login(username) {
    const res = await this.connection.invoke("RegisterUser", username);
    if (!res.success) {
      this.el("authError").textContent = res.error || "Ошибка входа";
      return;
    }
    this.me = res.user;
    this.rooms = res.rooms;
    this.el("selfName").textContent = this.me.username;
    this.el("selfAvatar").style.background = this.me.avatarColor;
    this.el("selfAvatar").textContent = this.me.username[0].toUpperCase();
    this.el("authModal").classList.add("hidden");
    this.el("appShell").classList.remove("hidden");
    this.renderRooms();
    if (this.rooms.length > 0) await this.joinRoom(this.rooms[0].name);
  },

  async joinRoom(name) {
    const res = await this.connection.invoke("JoinRoom", name);
    if (!res.success) return this.toast(res.error || "Не удалось войти", true);
    const snap = res.snapshot;
    this.activeRoom = { name: snap.name, description: snap.description, ownerUsername: snap.ownerUsername, isDefault: snap.isDefault };
    this.myRole = snap.currentUserRole;
    this.users = snap.users;
    this.bannedUsers = snap.bannedUsers;
    this.typingUsers.clear();
    this.el("messagesFeed").innerHTML = "";
    snap.messages.forEach((m) => this.appendMessage(m));
    this.renderRooms();
    this.renderHeader();
    this.renderMembers();
    this.renderTyping();
  },

  async sendMessage(text) {
    if (!this.activeRoom) return;
    const res = await this.connection.invoke("SendMessage", this.activeRoom.name, text);
    if (!res.success) this.toast(res.error || "Ошибка отправки", true);
  },

  async deleteMessage(id) {
    const res = await this.connection.invoke("DeleteMessage", this.activeRoom.name, id);
    if (!res.success) this.toast(res.error, true);
  },

  async kickUser(target) {
    const res = await this.connection.invoke("KickUser", this.activeRoom.name, target);
    if (!res.success) this.toast(res.error, true);
  },

  async banUser(target) {
    const res = await this.connection.invoke("BanUser", this.activeRoom.name, target);
    if (!res.success) this.toast(res.error, true);
  },

  async unbanUser(target) {
    const res = await this.connection.invoke("UnbanUser", this.activeRoom.name, target);
    if (!res.success) this.toast(res.error, true);
  },

  async muteUser(target, minutes) {
    const res = await this.connection.invoke("MuteUser", this.activeRoom.name, target, minutes);
    if (!res.success) this.toast(res.error, true);
  },

  async toggleMod(target) {
    const res = await this.connection.invoke("ToggleModerator", this.activeRoom.name, target);
    if (!res.success) this.toast(res.error, true);
  },

  openCreateRoomModal() {
    const modal = this.el("createRoomModal");
    modal.innerHTML = `<form id="createRoomForm" class="modal-card">
      <h2>Новая комната</h2>
      <input id="newRoomName" class="input" placeholder="Название комнаты" maxlength="30" required />
      <input id="newRoomDesc" class="input" placeholder="Описание (необязательно)" maxlength="100" />
      <div style="display:flex;gap:8px;justify-content:flex-end">
        <button type="button" id="cancelRoomBtn" class="btn btn-ghost">Отмена</button>
        <button type="submit" class="btn btn-primary">Создать</button>
      </div></form>`;
    modal.classList.remove("hidden");
    this.el("cancelRoomBtn").onclick = () => modal.classList.add("hidden");
    this.el("createRoomForm").onsubmit = async (e) => {
      e.preventDefault();
      const name = this.el("newRoomName").value.trim();
      const desc = this.el("newRoomDesc").value.trim();
      const res = await this.connection.invoke("CreateRoom", name, desc);
      if (!res.success) return this.toast(res.error, true);
      modal.classList.add("hidden");
      await this.joinRoom(name);
    };
  },

  openBansModal() {
    const modal = this.el("bansModal");
    modal.innerHTML = `<div class="modal-card"><h2 id="bansModalTitle"></h2>
      <div id="bansListBox" style="display:flex;flex-direction:column;gap:8px"></div>
      <button id="closeBansBtn" class="btn btn-ghost">Закрыть</button></div>`;
    this.el("bansModalTitle").textContent = `Забаненные в #${this.activeRoom.name}`;
    const box = this.el("bansListBox");
    if (!this.bannedUsers.length) {
      box.textContent = "Список пуст.";
    } else {
      for (const u of this.bannedUsers) {
        const row = document.createElement("div");
        row.style.cssText = "display:flex;justify-content:space-between;align-items:center";
        const span = document.createElement("span");
        span.textContent = u;
        const btn = this.makeModBtn("Разбанить", async () => { await this.unbanUser(u); modal.classList.add("hidden"); });
        row.append(span, btn);
        box.appendChild(row);
      }
    }
    modal.classList.remove("hidden");
    this.el("closeBansBtn").onclick = () => modal.classList.add("hidden");
  }
});
