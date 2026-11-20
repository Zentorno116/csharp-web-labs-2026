import { useState, type FormEvent } from "react";
import { api } from "../api/client";
import { useAuth } from "../context/AuthContext";
import type { ProjectDto } from "../types/api";

interface Props {
  projects: ProjectDto[];
  onChanged: () => Promise<void>;
  onError: (msg: string) => void;
}

export function ProjectsPanel({ projects, onChanged, onError }: Props) {
  const { user } = useAuth();
  const [editing, setEditing] = useState<ProjectDto | null>(null);
  const [name, setName] = useState("");
  const [description, setDescription] = useState("");
  const [status, setStatus] = useState("Active");

  const startEdit = (p: ProjectDto) => {
    setEditing(p); setName(p.name); setDescription(p.description); setStatus(p.status);
  };
  const reset = () => { setEditing(null); setName(""); setDescription(""); setStatus("Active"); };

  const submit = async (e: FormEvent) => {
    e.preventDefault();
    try {
      if (editing) await api.projects.update(editing.id, { name, description, status });
      else await api.projects.create({ name, description, status });
      reset();
      await onChanged();
    } catch (err) { onError(err instanceof Error ? err.message : "Ошибка"); }
  };

  const remove = async (p: ProjectDto) => {
    if (!confirm(`Удалить проект «${p.name}» и все его задачи?`)) return;
    try { await api.projects.remove(p.id); await onChanged(); }
    catch (err) { onError(err instanceof Error ? err.message : "Ошибка"); }
  };

  return (
    <div className="stack">
      <form className="card" onSubmit={submit}>
        <h3>{editing ? `Редактирование проекта #${editing.id}` : "Новый проект"}</h3>
        <div className="row" style={{ flexWrap: "wrap" }}>
          <input className="input" style={{ flex: 2, minWidth: 200 }} placeholder="Название проекта" value={name} onChange={(e) => setName(e.target.value)} required minLength={2} />
          <select className="select" style={{ flex: 1, minWidth: 140 }} value={status} onChange={(e) => setStatus(e.target.value)}>
            <option value="Active">Active</option>
            <option value="Completed">Completed</option>
            <option value="Archived">Archived</option>
          </select>
        </div>
        <textarea className="textarea" placeholder="Описание проекта" value={description} onChange={(e) => setDescription(e.target.value)} />
        <div className="row">
          <button className="btn btn-primary">{editing ? "Сохранить" : "+ Создать проект"}</button>
          {editing && <button type="button" className="btn btn-ghost" onClick={reset}>Отмена</button>}
        </div>
      </form>

      <div className="grid-3">
        {projects.map((p) => {
          const canManage = user?.role === "admin" || user?.id === p.ownerId;
          return (
            <div key={p.id} className="card">
              <div className="between">
                <strong>{p.name}</strong>
                <span className={`badge status-${p.status}`}>{p.status}</span>
              </div>
              <p className="muted">{p.description || "Без описания"}</p>
              <div className="between muted">
                <span>Владелец: {p.ownerUsername}</span>
                <span>Задач: {p.taskCount}</span>
              </div>
              {canManage && (
                <div className="row">
                  <button className="btn btn-ghost btn-sm" onClick={() => startEdit(p)}>Изменить</button>
                  <button className="btn btn-danger btn-sm" onClick={() => remove(p)}>Удалить</button>
                </div>
              )}
            </div>
          );
        })}
      </div>
    </div>
  );
}
