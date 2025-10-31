import React, { useEffect, useMemo, useState } from 'react';

type TaskItem = {
  id: string;
  description: string;
  isCompleted: boolean;
};

type Filter = 'all' | 'active' | 'completed';

const API_BASE = (import.meta as any).env?.VITE_API_BASE ?? 'http://localhost:5178';

async function api<T>(path: string, init?: RequestInit): Promise<T> {
  const res = await fetch(`${API_BASE}${path}`, {
    headers: { 'Content-Type': 'application/json' },
    ...init
  });
  if (!res.ok) throw new Error(await res.text());
  return res.status === 204 ? (undefined as unknown as T) : res.json();
}

export default function App() {
  const [tasks, setTasks] = useState<TaskItem[]>([]);
  const [newTask, setNewTask] = useState('');
  const [filter, setFilter] = useState<Filter>('all');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    (async () => {
      try {
        setLoading(true);
        const data = await api<TaskItem[]>('/api/tasks/');
        setTasks(data);
      } catch (e: any) {
        setError(e.message ?? 'Failed to load');
      } finally {
        setLoading(false);
      }
    })();
  }, []);

  const filtered = useMemo(() => {
    if (filter === 'active') return tasks.filter(t => !t.isCompleted);
    if (filter === 'completed') return tasks.filter(t => t.isCompleted);
    return tasks;
  }, [tasks, filter]);

  async function addTask() {
    const description = newTask.trim();
    if (!description) return;
    try {
      const created = await api<TaskItem>('/api/tasks/', {
        method: 'POST',
        body: JSON.stringify({ description, isCompleted: false })
      });
      setTasks(prev => [created, ...prev]);
      setNewTask('');
    } catch (e: any) {
      setError(e.message ?? 'Failed to add');
    }
  }

  async function toggleTask(task: TaskItem) {
    try {
      const updated = await api<TaskItem>(`/api/tasks/${task.id}`, {
        method: 'PUT',
        body: JSON.stringify({ ...task, isCompleted: !task.isCompleted })
      });
      setTasks(prev => prev.map(t => (t.id === task.id ? updated : t)));
    } catch (e: any) {
      setError(e.message ?? 'Failed to update');
    }
  }

  async function deleteTask(task: TaskItem) {
    try {
      await api<void>(`/api/tasks/${task.id}`, { method: 'DELETE' });
      setTasks(prev => prev.filter(t => t.id !== task.id));
    } catch (e: any) {
      setError(e.message ?? 'Failed to delete');
    }
  }

  return (
    <div className="app-container">
      <header className="header">
        <div className="brand">
          <img src="/src/assets/pathlock.png" alt="Pathlock" />
          <div className="titles">
            <h1>Pathlock Tasks</h1>
            <p className="subtitle">A clean, delightful task manager</p>
          </div>
        </div>
      </header>

      <main className="content">
        <section className="card">
          <div className="add-row">
            <input
              value={newTask}
              onChange={e => setNewTask(e.target.value)}
              placeholder="Add a new task..."
              onKeyDown={e => e.key === 'Enter' && addTask()}
            />
            <button className="primary" onClick={addTask}>Add</button>
          </div>

          <div className="toolbar">
            <div className="filters">
              <button
                className={filter === 'all' ? 'chip active' : 'chip'}
                onClick={() => setFilter('all')}
              >All</button>
              <button
                className={filter === 'active' ? 'chip active' : 'chip'}
                onClick={() => setFilter('active')}
              >Active</button>
              <button
                className={filter === 'completed' ? 'chip active' : 'chip'}
                onClick={() => setFilter('completed')}
              >Completed</button>
            </div>
            <div className="hint">API: {API_BASE}</div>
          </div>

          {error && <div className="alert">{error}</div>}
          {loading ? (
            <div className="loading">Loading tasks…</div>
          ) : (
            <ul className="list">
              {filtered.map(task => (
                <li key={task.id} className={task.isCompleted ? 'item done' : 'item'}>
                  <label className="checkbox">
                    <input
                      type="checkbox"
                      checked={task.isCompleted}
                      onChange={() => toggleTask(task)}
                    />
                    <span></span>
                  </label>
                  <span className="text">{task.description}</span>
                  <button className="ghost danger" onClick={() => deleteTask(task)}>
                    Delete
                  </button>
                </li>
              ))}
              {filtered.length === 0 && (
                <li className="empty">No tasks match this filter.</li>
              )}
            </ul>
          )}
        </section>
      </main>

      <footer className="footer">Built with ❤️ for Pathlock</footer>
    </div>
  );
}


