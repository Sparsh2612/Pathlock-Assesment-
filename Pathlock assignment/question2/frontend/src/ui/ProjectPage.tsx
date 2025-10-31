import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { api } from '../utils/auth';

type Project = { id: string; title: string; description?: string; createdAt: string };
type Task = { id: string; title: string; dueDate?: string; isCompleted: boolean; projectId: string };

export default function ProjectPage() {
  const { id } = useParams();
  const [project, setProject] = useState<Project | null>(null);
  const [tasks, setTasks] = useState<Task[]>([]);
  const [title, setTitle] = useState('');
  const [due, setDue] = useState<string>('');

  async function load() {
    const p = await api<Project>(`/api/projects/${id}`);
    setProject(p);
  }

  useEffect(() => { load(); }, [id]);

  async function add() {
    const t = title.trim(); if (!t) return;
    const created = await api<Task>(`/api/projects/${id}/tasks`, { method: 'POST', body: JSON.stringify({ title: t, dueDate: due || null }) });
    setTasks(prev => [created, ...prev]); setTitle(''); setDue('');
  }

  async function toggle(task: Task) {
    const updated = await api<Task>(`/api/tasks/${task.id}`, { method: 'PUT', body: JSON.stringify({ title: task.title, dueDate: task.dueDate ?? null, isCompleted: !task.isCompleted }) });
    setTasks(prev => prev.map(t => t.id === task.id ? updated : t));
  }

  async function remove(task: Task) {
    await api<void>(`/api/tasks/${task.id}`, { method: 'DELETE' });
    setTasks(prev => prev.filter(t => t.id !== task.id));
  }

  return (
    <section className="card">
      <h2>{project ? project.title : 'Project'}</h2>
      {project?.description && <p className="muted">{project.description}</p>}

      <div className="row" style={{marginTop:10}}>
        <input placeholder="Task title" value={title} onChange={e=>setTitle(e.target.value)} />
        <input type="date" value={due} onChange={e=>setDue(e.target.value)} style={{maxWidth:220}} />
        <button className="primary" onClick={add}>Add Task</button>
      </div>

      <ul className="list">
        {tasks.map(t => (
          <li className="item" key={t.id}>
            <label style={{display:'flex',alignItems:'center',gap:10,flex:1}}>
              <input type="checkbox" checked={t.isCompleted} onChange={()=>toggle(t)} />
              <span style={{textDecoration:t.isCompleted?'line-through':'none'}}>{t.title}</span>
            </label>
            <span className="muted" style={{width:160}}>{t.dueDate ? new Date(t.dueDate).toLocaleDateString() : 'No due date'}</span>
            <button className="danger" onClick={()=>remove(t)}>Delete</button>
          </li>
        ))}
        {tasks.length === 0 && <li className="item"><span className="muted">No tasks yet.</span></li>}
      </ul>
    </section>
  );
}


