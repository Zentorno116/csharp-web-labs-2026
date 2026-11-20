import { useState, type FormEvent } from "react";
import { api } from "../api/client";
import type { ProjectDto, TagDto, TaskItemDto, UserDto } from "../types/api";

interface Props {
  task: TaskItemDto | null;
  projects: ProjectDto[];
  users: UserDto[];
  tags: TagDto[];
  onClose: () => void;
  onSaved: () => Promise<void>;
  onError: (msg: string) => void;
}

export function TaskFormModal({ task, projects, users, tags, onClose, onSaved, onError }: Props) {
  const [title, setTitle] = useState(task?.title ?? "");
  const [description, setDescription] = useState(task?.description ?? "");
  const [status, setStatus] = useState(task?.status ?? "Todo");
  const [priority, setPriority] = useState(task?.priority ?? "Medium");
  const [dueDate, setDueDate] = useState(task?.dueDateUtc ? task.dueDateUtc.slice(0, 10) : "");
  const [projectId, setProjectId] = useState<number>(task?.projectId ?? projects[0]?.id ?? 0);
  const [assigneeId, setAssigneeId] = useState<string>(task?.assigneeId ? String(task.assigneeId) : "");
  const [tagIds, setTagIds] = useState<number[]>(task?.tags.map((t) => t.id) ?? []);

  const toggleTag = (id: number) =>
    setTagIds((prev) => (prev.includes(id) ? prev.filter((x) => x !== id) : [...prev, id]));

  const submit = async (e: FormEvent) => {
    e.preventDefault();
    const payload = {
      title, description, status, priority,
      dueDateUtc: dueDate ? new Date(dueDate).toISOString() : null,
      projectId,
      assigneeId: assigneeId ? Number(assigneeId) : null,
      tagIds
    };
    try {
      if (task) await api.tasks.update(task.id, payload);
      else await api.tasks.create(payload);
      await onSaved();
      onClose();
    } catch (err) { onError(err instanceof Error ? err.message : "Ошибка сохранения"); }
  };

  return (
    <div className="modal-backdrop" onClick={onClose}>
      <form className="modal-card" onClick={(e) => e.stopPropagation()} onSubmit={submit}>
        <h3>{task ? `Редактировать задачу #${task.id}` : "Новая задача"}</h3>
        <input className="input" placeholder="Заголовок задачи" value={title} onChange={(e) => setTitle(e.target.value)} required minLength={2} />
        <textarea className="textarea" placeholder="Описание" value={description} onChange={(e) => setDescription(e.target.value)} />
        <div className="row">
          <select className="select" value={projectId} onChange={(e) => setProjectId(Number(e.target.value))} required>
            {projects.map((p) => <option key={p.id} value={p.id}>{p.name}</option>)}
          </select>
          <select className="select" value={assigneeId} onChange={(e) => setAssigneeId(e.target.value)}>
            <option value="">Без исполнителя</option>
            {users.map((u) => <option key={u.id} value={u.id}>{u.username}</option>)}
          </select>
        </div>
        <div className="row">
          <select className="select" value={status} onChange={(e) => setStatus(e.target.value as TaskItemDto["status"])}>
            <option value="Todo">Todo</option><option value="InProgress">InProgress</option><option value="Done">Done</option>
          </select>
          <select className="select" value={priority} onChange={(e) => setPriority(e.target.value as TaskItemDto["priority"])}>
            <option value="Low">Low</option><option value="Medium">Medium</option><option value="High">High</option>
          </select>
          <input className="input" type="date" value={dueDate} onChange={(e) => setDueDate(e.target.value)} />
        </div>
        <div className="stack">
          <span className="muted">Теги:</span>
          <div className="row" style={{ flexWrap: "wrap" }}>
            {tags.map((t) => (
              <button key={t.id} type="button" className={`tag-pill ${tagIds.includes(t.id) ? "selected" : ""}`}
                style={{ background: t.color }} onClick={() => toggleTag(t.id)}>{t.name}</button>
            ))}
          </div>
        </div>
        <div className="row" style={{ justifyContent: "flex-end" }}>
          <button type="button" className="btn btn-ghost" onClick={onClose}>Отмена</button>
          <button className="btn btn-primary">Сохранить</button>
        </div>
      </form>
    </div>
  );
}
