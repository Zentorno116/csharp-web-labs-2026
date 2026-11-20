import type { AuthResponseDto, CommentDto, ProjectDto, TagDto, TaskItemDto, UserDto } from "../types/api";

const API_BASE = import.meta.env.VITE_API_BASE ?? "";
const TOKEN_KEY = "taskflow_jwt";

export const tokenStore = {
  get: () => localStorage.getItem(TOKEN_KEY),
  set: (t: string) => localStorage.setItem(TOKEN_KEY, t),
  clear: () => localStorage.removeItem(TOKEN_KEY)
};

async function request<T>(path: string, init: RequestInit = {}): Promise<T> {
  const token = tokenStore.get();
  const headers: Record<string, string> = {
    "Content-Type": "application/json",
    ...(init.headers as Record<string, string> | undefined)
  };
  if (token) headers.Authorization = `Bearer ${token}`;

  const res = await fetch(`${API_BASE}${path}`, { ...init, headers });
  if (res.status === 204) return undefined as T;

  const data = await res.json().catch(() => null);
  if (!res.ok) {
    if (res.status === 401) tokenStore.clear();
    const msg = data?.error || data?.title || `Ошибка HTTP ${res.status}`;
    throw new Error(msg);
  }
  return data as T;
}

export const api = {
  auth: {
    register: (body: { username: string; email: string; password: string }) =>
      request<AuthResponseDto>("/api/auth/register", { method: "POST", body: JSON.stringify(body) }),
    login: (body: { login: string; password: string }) =>
      request<AuthResponseDto>("/api/auth/login", { method: "POST", body: JSON.stringify(body) }),
    me: () => request<UserDto>("/api/auth/me")
  },
  users: {
    list: () => request<UserDto[]>("/api/users"),
    updateRole: (id: number, role: "admin" | "user") =>
      request<UserDto>(`/api/users/${id}/role`, { method: "PUT", body: JSON.stringify({ role }) }),
    remove: (id: number) => request<void>(`/api/users/${id}`, { method: "DELETE" })
  },
  projects: {
    list: () => request<ProjectDto[]>("/api/projects"),
    create: (body: { name: string; description: string; status: string }) =>
      request<ProjectDto>("/api/projects", { method: "POST", body: JSON.stringify(body) }),
    update: (id: number, body: { name: string; description: string; status: string }) =>
      request<ProjectDto>(`/api/projects/${id}`, { method: "PUT", body: JSON.stringify(body) }),
    remove: (id: number) => request<void>(`/api/projects/${id}`, { method: "DELETE" })
  },
  tasks: {
    list: (params?: { projectId?: number; status?: string; tagId?: number }) => {
      const qs = new URLSearchParams();
      if (params?.projectId) qs.set("projectId", String(params.projectId));
      if (params?.status) qs.set("status", params.status);
      if (params?.tagId) qs.set("tagId", String(params.tagId));
      const q = qs.toString();
      return request<TaskItemDto[]>(`/api/tasks${q ? `?${q}` : ""}`);
    },
    create: (body: Record<string, unknown>) =>
      request<TaskItemDto>("/api/tasks", { method: "POST", body: JSON.stringify(body) }),
    update: (id: number, body: Record<string, unknown>) =>
      request<TaskItemDto>(`/api/tasks/${id}`, { method: "PUT", body: JSON.stringify(body) }),
    remove: (id: number) => request<void>(`/api/tasks/${id}`, { method: "DELETE" })
  },
  comments: {
    byTask: (taskId: number) => request<CommentDto[]>(`/api/comments/task/${taskId}`),
    create: (body: { taskItemId: number; content: string }) =>
      request<CommentDto>("/api/comments", { method: "POST", body: JSON.stringify(body) }),
    update: (id: number, content: string) =>
      request<CommentDto>(`/api/comments/${id}`, { method: "PUT", body: JSON.stringify({ content }) }),
    remove: (id: number) => request<void>(`/api/comments/${id}`, { method: "DELETE" })
  },
  tags: {
    list: () => request<TagDto[]>("/api/tags"),
    create: (body: { name: string; color: string }) =>
      request<TagDto>("/api/tags", { method: "POST", body: JSON.stringify(body) }),
    update: (id: number, body: { name: string; color: string }) =>
      request<TagDto>(`/api/tags/${id}`, { method: "PUT", body: JSON.stringify(body) }),
    remove: (id: number) => request<void>(`/api/tags/${id}`, { method: "DELETE" })
  }
};
