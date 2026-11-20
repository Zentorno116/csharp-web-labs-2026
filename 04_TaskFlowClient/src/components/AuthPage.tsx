import { useState, type FormEvent } from "react";
import { useAuth } from "../context/AuthContext";

export function AuthPage() {
  const { login, register } = useAuth();
  const [mode, setMode] = useState<"login" | "register">("login");
  const [username, setUsername] = useState("");
  const [email, setEmail] = useState("");
  const [loginVal, setLoginVal] = useState("admin@taskflow.local");
  const [password, setPassword] = useState("Admin123!");
  const [error, setError] = useState("");
  const [busy, setBusy] = useState(false);

  const onSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setError("");
    setBusy(true);
    try {
      if (mode === "login") await login(loginVal, password);
      else await register(username, email, password);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Ошибка авторизации");
    } finally {
      setBusy(false);
    }
  };

  const fillDemo = (l: string, p: string) => { setMode("login"); setLoginVal(l); setPassword(p); };

  return (
    <div className="auth-wrap">
      <form className="card auth-card" onSubmit={onSubmit}>
        <div className="between">
          <h2>⚡ TaskFlow</h2>
          <a href="/swagger/index.html" target="_blank" rel="noreferrer">Swagger API ↗</a>
        </div>
        <p className="muted">ASP.NET Core 10 + PostgreSQL + JWT + React</p>
        <div className="row">
          <button type="button" className={`nav-tab ${mode === "login" ? "active" : ""}`} onClick={() => setMode("login")}>Вход</button>
          <button type="button" className={`nav-tab ${mode === "register" ? "active" : ""}`} onClick={() => setMode("register")}>Регистрация</button>
        </div>

        {mode === "register" ? (
          <>
            <input className="input" placeholder="Имя пользователя" value={username} onChange={(e) => setUsername(e.target.value)} required minLength={2} />
            <input className="input" type="email" placeholder="Email" value={email} onChange={(e) => setEmail(e.target.value)} required />
          </>
        ) : (
          <input className="input" placeholder="Email или логин" value={loginVal} onChange={(e) => setLoginVal(e.target.value)} required />
        )}
        <input className="input" type="password" placeholder="Пароль (мин. 6 симв.)" value={password} onChange={(e) => setPassword(e.target.value)} required minLength={6} />

        {error && <div className="error-banner">{error}</div>}
        <button className="btn btn-primary" disabled={busy}>
          {busy ? "Загрузка..." : mode === "login" ? "Войти по JWT" : "Создать аккаунт"}
        </button>

        <div className="stack" style={{ marginTop: 6 }}>
          <span className="muted">Демо-аккаунты:</span>
          <div className="row">
            <button type="button" className="btn btn-ghost btn-sm" onClick={() => fillDemo("admin@taskflow.local", "Admin123!")}>👑 Admin</button>
            <button type="button" className="btn btn-ghost btn-sm" onClick={() => fillDemo("timur@taskflow.local", "User123!")}>👤 User (timur)</button>
          </div>
        </div>
      </form>
    </div>
  );
}
