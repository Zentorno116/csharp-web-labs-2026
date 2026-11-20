"use strict";

window.ChatApp = {
  connection: null,
  me: null,
  rooms: [],
  activeRoom: null,
  myRole: "User",
  users: [],
  bannedUsers: [],
  typingUsers: new Set(),
  typingTimer: null,

  el: (id) => document.getElementById(id),

  roleRank(role) {
    if (role === "Owner") return 2;
    if (role === "Moderator") return 1;
    return 0;
  },

  roleLabel(role) {
    if (role === "Owner") return "👑 Владелец";
    if (role === "Moderator") return "🛡️ Модератор";
    return "Участник";
  },

  formatTime(iso) {
    return new Date(iso).toLocaleTimeString("ru-RU", { hour: "2-digit", minute: "2-digit" });
  },

  toast(text, isError = false) {
    const box = this.el("toastContainer");
    const node = document.createElement("div");
    node.className = "toast" + (isError ? " error" : "");
    node.textContent = text;
    box.appendChild(node);
    setTimeout(() => node.remove(), 3500);
  }
};
