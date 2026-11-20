import { useCallback, useEffect, useState } from "react";
import { api } from "./api/client";
import { AuthPage } from "./components/AuthPage";
import { Navbar, type TabKey } from "./components/Navbar";
import { ProjectsPanel } from "./components/ProjectsPanel";
import { TagsPanel } from "./components/TagsPanel";
import { TasksPanel } from "./components/TasksPanel";
import { UsersPanel } from "./components/UsersPanel";
import { AuthProvider, useAuth } from "./context/AuthContext";
import type { ProjectDto, TagDto, TaskItemDto, UserDto } from "./types/api";

function Dashboard() {
  const { user, loading } = useAuth();
  const [tab, setTab] = useState<TabKey>("tasks");
  const [projects, setProjects] = useState<ProjectDto[]>([]);
  const [tasks, setTasks] = useState<TaskItemDto[]>([]);
  const [tags, setTags] = useState<TagDto[]>([]);
  const [users, setUsers] = useState<UserDto[]>([]);
  const [error, setError] = useState("");

  const loadAll = useCallback(async () => {
    if (!user) return;
    try {
      const [p, t, tg, u] = await Promise.all([
        api.projects.list(), api.tasks.list(), api.tags.list(), api.users.list()
      ]);
      setProjects(p); setTasks(t); setTags(tg); setUsers(u);
      setError("");
    } catch (err) {
      setError(err instanceof Error ? err.message : "Ошибка загрузки данных");
    }
  }, [user]);

  useEffect(() => { loadAll(); }, [loadAll]);

  if (loading) return <div className="auth-wrap">Загрузка...</div>;
  if (!user) return <AuthPage />;

  return (
    <>
      <Navbar tab={tab} onTab={setTab} />
      <main className="container stack">
        {error && (
          <div className="error-banner between">
            <span>{error}</span>
            <button className="btn btn-ghost btn-sm" onClick={() => setError("")}>✕</button>
          </div>
        )}
        {tab === "tasks" && <TasksPanel tasks={tasks} projects={projects} users={users} tags={tags} onChanged={loadAll} onError={setError} />}
        {tab === "projects" && <ProjectsPanel projects={projects} onChanged={loadAll} onError={setError} />}
        {tab === "tags" && <TagsPanel tags={tags} onChanged={loadAll} onError={setError} />}
        {tab === "users" && <UsersPanel users={users} onChanged={loadAll} onError={setError} />}
      </main>
    </>
  );
}

export default function App() {
  return (
    <AuthProvider>
      <Dashboard />
    </AuthProvider>
  );
}
