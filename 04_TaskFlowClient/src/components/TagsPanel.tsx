import { useState, type FormEvent } from "react";
import { api } from "../api/client";
import { useAuth } from "../context/AuthContext";
import type { TagDto } from "../types/api";

interface Props {
  tags: TagDto[];
  onChanged: () => Promise<void>;
  onError: (msg: string) => void;
}

export function TagsPanel({ tags, onChanged, onError }: Props) {
  const { user } = useAuth();
  const [editing, setEditing] = useState<TagDto | null>(null);
  const [name, setName] = useState("");
  const [color, setColor] = useState("#6366f1");

  const startEdit = (t: TagDto) => { setEditing(t); setName(t.name); setColor(t.color); };
  const reset = () => { setEditing(null); setName(""); setColor("#6366f1"); };

  const submit = async (e: FormEvent) => {
    e.preventDefault();
    try {
      if (editing) await api.tags.update(editing.id, { name, color });
      else await api.tags.create({ name, color });
      reset();
      await onChanged();
    } catch (err) { onError(err instanceof Error ? err.message : "Ошибка"); }
  };

  const remove = async (t: TagDto) => {
    if (!confirm(`Удалить тег «${t.name}»?`)) return;
    try { await api.tags.remove(t.id); await onChanged(); }
    catch (err) { onError(err instanceof Error ? err.message : "Ошибка"); }
  };

  return (
    <div className="stack">
      <form className="card" onSubmit={submit}>
        <h3>{editing ? `Редактировать тег #${editing.id}` : "Новый тег"}</h3>
        <div className="row" style={{ flexWrap: "wrap" }}>
          <input className="input" style={{ flex: 2, minWidth: 180 }} placeholder="Название тега" value={name} onChange={(e) => setName(e.target.value)} required />
          <input type="color" value={color} onChange={(e) => setColor(e.target.value)} style={{ width: 52, height: 40, border: "none", background: "none", cursor: "pointer" }} />
          <button className="btn btn-primary">{editing ? "Сохранить" : "+ Добавить тег"}</button>
          {editing && <button type="button" className="btn btn-ghost" onClick={reset}>Отмена</button>}
        </div>
      </form>

      <div className="grid-3">
        {tags.map((t) => (
          <div key={t.id} className="card between" style={{ flexDirection: "row" }}>
            <span className="tag-pill" style={{ background: t.color }}>{t.name}</span>
            <div className="row">
              <button className="btn btn-ghost btn-sm" onClick={() => startEdit(t)}>Изменить</button>
              {user?.role === "admin" && (
                <button className="btn btn-danger btn-sm" onClick={() => remove(t)}>Удалить</button>
              )}
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}
