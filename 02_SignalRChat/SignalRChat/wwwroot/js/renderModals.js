"use strict";

Object.assign(window.ChatApp, {
  renderHeader() {
    const r = this.activeRoom;
    this.el("activeRoomName").textContent = r ? "# " + r.name : "# Выберите комнату";
    this.el("activeRoomDesc").textContent = r ? `${r.description} · Владелец: ${r.ownerUsername}` : "";

    const badge = this.el("myRoleBadge");
    if (r) {
      badge.className = `role-badge role-${this.myRole}`;
      badge.textContent = this.roleLabel(this.myRole);
      badge.classList.remove("hidden");
    } else badge.classList.add("hidden");

    const isMod = this.roleRank(this.myRole) >= 1;
    const isOwner = this.myRole === "Owner";
    this.el("openBansBtn").classList.toggle("hidden", !r || !isMod);
    this.el("bansCount").textContent = String(this.bannedUsers.length);
    this.el("deleteRoomBtn").classList.toggle("hidden", !r || !isOwner || r.isDefault);
    this.el("messageInput").disabled = !r;
    this.el("sendBtn").disabled = !r;
  },

  renderMembers() {
    this.el("membersCount").textContent = String(this.users.length);
    const list = this.el("membersList");
    list.innerHTML = "";
    const myRank = this.roleRank(this.myRole);

    for (const u of this.users) {
      const card = document.createElement("div");
      card.className = "member-card";
      const top = document.createElement("div");
      top.className = "member-top";

      const av = document.createElement("div");
      av.className = "avatar";
      av.style.background = u.avatarColor;
      av.textContent = u.username[0].toUpperCase();

      const info = document.createElement("div");
      const name = document.createElement("div");
      name.className = "user-name";
      name.textContent = u.username + (u.username === this.me?.username ? " (вы)" : "");
      info.appendChild(name);

      if (u.role !== "User") {
        const rb = document.createElement("span");
        rb.className = `role-badge role-${u.role}`;
        rb.textContent = this.roleLabel(u.role);
        info.appendChild(rb);
      }
      if (u.isMuted) {
        const mb = document.createElement("span");
        mb.className = "role-badge role-Muted";
        mb.textContent = "🔇 Мут";
        info.appendChild(mb);
      }
      top.append(av, info);
      card.appendChild(top);

      const isSelf = u.username.toLowerCase() === this.me?.username.toLowerCase();
      if (!isSelf && myRank >= 1 && myRank > this.roleRank(u.role)) {
        const bar = document.createElement("div");
        bar.className = "mod-actions";
        if (this.myRole === "Owner") {
          bar.appendChild(this.makeModBtn(u.role === "Moderator" ? "Снять мод." : "Модер", () => this.toggleMod(u.username)));
        }
        bar.appendChild(this.makeModBtn(u.isMuted ? "Размут" : "Мут 5м", () => this.muteUser(u.username, u.isMuted ? 0 : 5)));
        bar.appendChild(this.makeModBtn("Кик", () => this.kickUser(u.username), true));
        bar.appendChild(this.makeModBtn("Бан", () => this.banUser(u.username), true));
        card.appendChild(bar);
      }
      list.appendChild(card);
    }
  },

  makeModBtn(label, onClick, danger = false) {
    const b = document.createElement("button");
    b.className = "mod-btn" + (danger ? " danger" : "");
    b.textContent = label;
    b.onclick = onClick;
    return b;
  },

  renderTyping() {
    const arr = Array.from(this.typingUsers);
    this.el("typingBar").textContent = arr.length ? `${arr.join(", ")} печатает...` : "";
  }
});
