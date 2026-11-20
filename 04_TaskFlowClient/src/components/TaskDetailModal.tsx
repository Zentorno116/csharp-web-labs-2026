import { useEffect, useState, type FormEvent } from "react";
import { api } from "../api/client";
import { useAuth } from "../context/AuthContext";
import type { CommentDto, TaskItemDto } from "../types/api";

interface Props {
  task: TaskItemDto;
  onClose: () => void;
  onChanged: () => Promise<void>;
  onError: (msg: string) => void;
}

export function TaskDetailModal({ task, onClose, onChanged, onError }: Props) {
  const { user } = useAuth();
  const [comments, setComments] = useState<CommentDto[]>([]);
  const [text, setText] = useState("");
  const [editId, setEditId] = useState<number | null>(null);
  const [editText, setEditText] = useState("");

  const loadComments = async () => {
    try { setComments(await api.comments.byTask(task.id)); }
    catch (err) { onError(err instanceof Error ? err.message : "Ошибка загрузки комментариев"); }
  };

  useEffect(() => { loadComments(); }, [task.id]);

  const add = async (e: FormEvent) => {
    e.preventDefault();
    if (!text.trim()) return;
    try {
      await api.comments.create({ taskItemId: task.id, content: text });
      setText("");
      await loadComments();
      await onChanged();
    } catch (err) { onError(err instanceof Error ? err.message : "Ошибка"); }
  };

  const saveEdit = async (id: number) => {
    try {
      await api.comments.update(id, editText);
      setEditId(null);
      await loadComments();
    } catch (err) { onError(err instanceof Error ? err.message : "Ошибка"); }
  };

  const remove = async (id: number) => {
    try {
      await api.comments.remove(id);
      await loadComments();
      await onChanged();
    } catch (err) { onError(err instanceof Error ? err.message : "Ошибка"); }
  };

  return (
    <div className="modal-backdrop" onClick={onClose}>
      <div className="modal-card" onClick={(e) => e.stopPropagation()}>
        <div className="between">
          <h3>#{task.id} {task.title}</h3>
          <button className="btn btn-ghost btn-sm" onClick={onClose}>✕</button>
        </div>
        <p className="muted">{task.description || "Без описания"}</p>
        <div className="row" style={{ flexWrap: "wrap" }}>
          <span className={`badge status-${task.status}`}>{task.status}</span>
          <span className={`badge prio-${task.priority}`}>{task.priority}</span>
          <span className="muted">Проект: {task.projectName}</span>
          {task.assigneeUsername && <span className="muted">Исполнитель: {task.assigneeUsername}</span>}
        </div>
        <hr style={{ borderColor: "var(--border)" }} />
        <h4>Комментарии ({comments.length})</h4>
        <div className="stack">
          {comments.map((c) => {
            const canManage = user?.role === "admin" || user?.id === c.authorId;
            return (
              <div key={c.id} className="card" style={{ padding: 12 }}>
                <div className="between muted">
                  <strong>{c.authorUsername}</strong>
                  <span>{new Date(c.createdAtUtc).toLocaleString("ru-RU")}</span>
                </div>
                {editId === c.id ? (
                  <div className="row">
                    <input className="input" value={editText} onChange={(e) => setEditText(e.target.value)} />
                    <button className="btn btn-primary btn-sm" onClick={() => saveEdit(c.id)}>Ок</button>
                    <button className="btn btn-ghost btn-sm" onClick={() => setEditId(null)}>Отмена</button>
                  </div>
                ) : (
                  <p>{c.content}</p>
                )}
                {canManage && editId !== c.id && (
                  <div className="row">
                    <button className="btn btn-ghost btn-sm" onClick={() => { setEditId(c.id); setEditText(c.content); }}>Изменить</button>
                    <button className="btn btn-danger btn-sm" onClick={() => remove(c.id)}>Удалить</button>
                  </div>
                )}
              </div>
            );
          })}
        </div>
        <form className="row" onSubmit={add}>
          <input className="input" placeholder="Добавить комментарий..." value={text} onChange={(e) => setText(e.target.value)} required />
          <button className="btn btn-primary">Отправить</button>
        </form>
      </div>
    </div>
  );
}
