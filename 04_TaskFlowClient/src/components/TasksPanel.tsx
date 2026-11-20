import { useState } from "react";
import { api } from "../api/client";
import type { ProjectDto, TagDto, TaskItemDto, UserDto } from "../types/api";
import { TaskDetailModal } from "./TaskDetailModal";
import { TaskFormModal } from "./TaskFormModal";

interface Props {
  tasks: TaskItemDto[];
  projects: ProjectDto[];
  users: UserDto[];
  tags: TagDto[];
  onChanged: () => Promise<void>;
  onError: (msg: string) => void;
}

export function TasksPanel({ tasks, projects, users, tags, onChanged, onError }: Props) {
  const [filterProj, setFilterProj] = useState("");
  const [filterTag, setFilterTag] = useState("");
  const [search, setSearch] = useState("");
  const [formTask, setFormTask] = useState<TaskItemDto | null | undefined>(undefined);
  const [detailTask, setDetailTask] = useState<TaskItemDto | null>(null);

  const filtered = tasks.filter((t) =>
    (!filterProj || t.projectId === Number(filterProj)) &&
    (!filterTag || t.tags.some((tg) => tg.id === Number(filterTag))) &&
    (!search || t.title.toLowerCase().includes(search.toLowerCase()) || t.description.toLowerCase().includes(search.toLowerCase()))
  );

  const moveStatus = async (t: TaskItemDto, status: TaskItemDto["status"]) => {
    try {
      await api.tasks.update(t.id, {
        title: t.title, description: t.description, status, priority: t.priority,
        dueDateUtc: t.dueDateUtc, projectId: t.projectId, assigneeId: t.assigneeId,
        tagIds: t.tags.map((x) => x.id)
      });
      await onChanged();
    } catch (err) { onError(err instanceof Error ? err.message : "Ошибка"); }
  };

  const remove = async (t: TaskItemDto) => {
    if (!confirm(`Удалить задачу «${t.title}»?`)) return;
    try { await api.tasks.remove(t.id); await onChanged(); }
    catch (err) { onError(err instanceof Error ? err.message : "Ошибка"); }
  };

  const cols: { key: TaskItemDto["status"]; title: string }[] = [
    { key: "Todo", title: "📌 К выполнению" },
    { key: "InProgress", title: "⚙️ В работе" },
    { key: "Done", title: "✅ Готово" }
  ];

  return (
    <div className="stack">
      <div className="between">
        <div className="row" style={{ flexWrap: "wrap", flex: 1 }}>
          <input className="input" style={{ maxWidth: 240 }} placeholder="Поиск задач..." value={search} onChange={(e) => setSearch(e.target.value)} />
          <select className="select" style={{ maxWidth: 220 }} value={filterProj} onChange={(e) => setFilterProj(e.target.value)}>
            <option value="">Все проекты</option>
            {projects.map((p) => <option key={p.id} value={p.id}>{p.name}</option>)}
          </select>
          <select className="select" style={{ maxWidth: 180 }} value={filterTag} onChange={(e) => setFilterTag(e.target.value)}>
            <option value="">Все теги</option>
            {tags.map((tg) => <option key={tg.id} value={tg.id}>{tg.name}</option>)}
          </select>
        </div>
        <button className="btn btn-primary" disabled={!projects.length} onClick={() => setFormTask(null)}>+ Новая задача</button>
      </div>

      <div className="kanban">
        {cols.map((col) => {
          const items = filtered.filter((t) => t.status === col.key);
          return (
            <div key={col.key} className="kanban-col">
              <div className="between"><strong>{col.title}</strong><span className="muted">{items.length}</span></div>
              {items.map((t) => (
                <div key={t.id} className="card" style={{ padding: 14 }}>
                  <div className="between">
                    <strong>{t.title}</strong>
                    <span className={`badge prio-${t.priority}`}>{t.priority}</span>
                  </div>
                  <div className="muted">{t.projectName}{t.assigneeUsername ? ` · @${t.assigneeUsername}` : ""}</div>
                  {t.tags.length > 0 && (
                    <div className="row" style={{ flexWrap: "wrap" }}>
                      {t.tags.map((tg) => <span key={tg.id} className="tag-pill" style={{ background: tg.color }}>{tg.name}</span>)}
                    </div>
                  )}
                  <div className="row" style={{ flexWrap: "wrap" }}>
                    <button className="btn btn-ghost btn-sm" onClick={() => setDetailTask(t)}>💬 {t.commentCount}</button>
                    <button className="btn btn-ghost btn-sm" onClick={() => setFormTask(t)}>✏️</button>
                    {col.key !== "Done" && (
                      <button className="btn btn-ghost btn-sm" onClick={() => moveStatus(t, col.key === "Todo" ? "InProgress" : "Done")}>→</button>
                    )}
                    <button className="btn btn-danger btn-sm" onClick={() => remove(t)}>🗑</button>
                  </div>
                </div>
              ))}
            </div>
          );
        })}
      </div>

      {formTask !== undefined && (
        <TaskFormModal task={formTask} projects={projects} users={users} tags={tags}
          onClose={() => setFormTask(undefined)} onSaved={onChanged} onError={onError} />
      )}
      {detailTask && (
        <TaskDetailModal task={detailTask} onClose={() => setDetailTask(null)} onChanged={onChanged} onError={onError} />
      )}
    </div>
  );
}
