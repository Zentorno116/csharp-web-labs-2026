"use strict";

Object.assign(window.ChatApp, {
  renderRooms() {
    const list = this.el("roomsList");
    list.innerHTML = "";
    for (const r of this.rooms) {
      const item = document.createElement("div");
      item.className = "room-item" + (this.activeRoom?.name === r.name ? " active" : "");
      item.onclick = () => this.joinRoom(r.name);

      const left = document.createElement("div");
      const name = document.createElement("div");
      name.className = "room-item-name";
      name.textContent = "# " + r.name;
      left.appendChild(name);

      const count = document.createElement("span");
      count.className = "room-count";
      count.textContent = String(r.onlineCount);

      item.append(left, count);
      list.appendChild(item);
    }
  },

  appendMessage(m) {
    const feed = this.el("messagesFeed");
    if (m.isSystem) {
      const sys = document.createElement("div");
      sys.className = "msg-system";
      sys.textContent = `${m.text} · ${this.formatTime(m.timestampUtc)}`;
      feed.appendChild(sys);
      feed.scrollTop = feed.scrollHeight;
      return;
    }

    const isOwn = this.me && m.sender.toLowerCase() === this.me.username.toLowerCase();
    const row = document.createElement("div");
    row.className = "msg-row" + (isOwn ? " own" : "");
    row.dataset.msgId = m.id;

    const av = document.createElement("div");
    av.className = "avatar";
    av.style.background = m.avatarColor;
    av.textContent = m.sender[0].toUpperCase();

    const bubble = document.createElement("div");
    bubble.className = "msg-bubble";

    const meta = document.createElement("div");
    meta.className = "msg-meta";
    const author = document.createElement("strong");
    author.textContent = m.sender;
    meta.appendChild(author);

    if (m.senderRole !== "User") {
      const badge = document.createElement("span");
      badge.className = `role-badge role-${m.senderRole}`;
      badge.textContent = this.roleLabel(m.senderRole);
      meta.appendChild(badge);
    }

    const time = document.createElement("span");
    time.className = "muted";
    time.textContent = this.formatTime(m.timestampUtc);
    meta.appendChild(time);

    const canDelete = !m.isDeleted && (isOwn ||
      (this.roleRank(this.myRole) >= 1 && this.roleRank(this.myRole) > this.roleRank(m.senderRole)));
    if (canDelete) {
      const del = document.createElement("button");
      del.className = "msg-del-btn";
      del.title = "Удалить сообщение";
      del.textContent = "✕";
      del.onclick = () => this.deleteMessage(m.id);
      meta.appendChild(del);
    }

    const body = document.createElement("div");
    body.className = "msg-text" + (m.isDeleted ? " msg-deleted" : "");
    body.textContent = m.text;

    bubble.append(meta, body);
    row.append(av, bubble);
    feed.appendChild(row);
    feed.scrollTop = feed.scrollHeight;
  }
});
