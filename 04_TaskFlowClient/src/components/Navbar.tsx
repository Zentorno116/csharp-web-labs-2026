import { useAuth } from "../context/AuthContext";

export type TabKey = "tasks" | "projects" | "tags" | "users";

interface Props {
  tab: TabKey;
  onTab: (t: TabKey) => void;
}

export function Navbar({ tab, onTab }: Props) {
  const { user, logout } = useAuth();
  const tabs: { key: TabKey; label: string }[] = [
    { key: "tasks", label: "📋 Задачи" },
    { key: "projects", label: "📁 Проекты" },
    { key: "tags", label: "🏷 Теги" },
    { key: "users", label: "👥 Пользователи" }
  ];

  return (
    <header className="navbar between">
      <div className="row">
        <span className="brand">⚡ TaskFlow</span>
        <nav className="nav-tabs">
          {tabs.map((t) => (
            <button key={t.key} className={`nav-tab ${tab === t.key ? "active" : ""}`} onClick={() => onTab(t.key)}>
              {t.label}
            </button>
          ))}
        </nav>
      </div>
      <div className="row">
        <a className="btn btn-ghost btn-sm" href="/swagger/index.html" target="_blank" rel="noreferrer">Swagger ↗</a>
        {user && (
          <>
            <span className={`badge role-${user.role}`}>{user.role === "admin" ? "👑 admin" : "user"}</span>
            <strong>{user.username}</strong>
            <button className="btn btn-ghost btn-sm" onClick={logout}>Выйти</button>
          </>
        )}
      </div>
    </header>
  );
}
