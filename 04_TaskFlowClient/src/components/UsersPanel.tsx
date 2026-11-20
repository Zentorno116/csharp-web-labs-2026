import { api } from "../api/client";
import { useAuth } from "../context/AuthContext";
import type { UserDto } from "../types/api";

interface Props {
  users: UserDto[];
  onChanged: () => Promise<void>;
  onError: (msg: string) => void;
}

export function UsersPanel({ users, onChanged, onError }: Props) {
  const { user: me } = useAuth();
  const isAdmin = me?.role === "admin";

  const toggleRole = async (u: UserDto) => {
    try {
      await api.users.updateRole(u.id, u.role === "admin" ? "user" : "admin");
      await onChanged();
    } catch (e) { onError(e instanceof Error ? e.message : "Ошибка"); }
  };

  const remove = async (u: UserDto) => {
    if (!confirm(`Удалить пользователя ${u.username}?`)) return;
    try {
      await api.users.remove(u.id);
      await onChanged();
    } catch (e) { onError(e instanceof Error ? e.message : "Ошибка"); }
  };

  return (
    <div className="stack">
      <div className="between">
        <h2>Пользователи ({users.length})</h2>
        {!isAdmin && <span className="muted">Управление ролями доступно роли admin</span>}
      </div>
      <table className="table">
        <thead>
          <tr><th>ID</th><th>Имя</th><th>Email</th><th>Роль</th><th>Создан</th>{isAdmin && <th>Действия</th>}</tr>
        </thead>
        <tbody>
          {users.map((u) => (
            <tr key={u.id}>
              <td>#{u.id}</td>
              <td><strong>{u.username}</strong></td>
              <td>{u.email}</td>
              <td><span className={`badge role-${u.role}`}>{u.role}</span></td>
              <td className="muted">{new Date(u.createdAtUtc).toLocaleDateString("ru-RU")}</td>
              {isAdmin && (
                <td>
                  <div className="row">
                    <button className="btn btn-ghost btn-sm" onClick={() => toggleRole(u)}>
                      Сделать {u.role === "admin" ? "user" : "admin"}
                    </button>
                    {u.id !== me?.id && (
                      <button className="btn btn-danger btn-sm" onClick={() => remove(u)}>Удалить</button>
                    )}
                  </div>
                </td>
              )}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
