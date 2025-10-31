import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { api } from '../utils/auth';

type Project = { id: string; title: string; description?: string; createdAt: string };

export default function DashboardPage() {
  const [projects, setProjects] = useState<Project[]>([]);
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');

  async function load() {
    const data = await api<Project[]>(`/api/projects`);
    setProjects(data);
  }

  useEffect(() => { load(); }, []);

  async function create() {
    const t = title.trim(); if (!t) return;
    const p = await api<Project>(`/api/projects`, { method: 'POST', body: JSON.stringify({ title: t, description }) });
    setProjects(prev => [p, ...prev]); setTitle(''); setDescription('');
  }

  async function remove(id: string) {
    await api<void>(`/api/projects/${id}`, { method: 'DELETE' });
    setProjects(prev => prev.filter(p => p.id !== id));
  }

  return (
    <section className="card">
      <h2>Your Projects</h2>
      <div className="row"><input placeholder="Project title" value={title} onChange={e=>setTitle(e.target.value)} /></div>
      <div className="row" style={{marginTop:8}}><input placeholder="Description (optional)" value={description} onChange={e=>setDescription(e.target.value)} /></div>
      <div className="row" style={{marginTop:10}}><button className="primary" onClick={create}>Create Project</button></div>

      <ul className="list">
        {projects.map(p => (
          <li className="item" key={p.id}>
            <div style={{flex:1}}>
              <Link to={`/projects/${p.id}`}>{p.title}</Link>
              <div className="muted" style={{fontSize:12}}>{new Date(p.createdAt).toLocaleString()}</div>
            </div>
            <button className="danger" onClick={() => remove(p.id)}>Delete</button>
          </li>
        ))}
        {projects.length === 0 && <li className="item"><span className="muted">No projects yet.</span></li>}
      </ul>
    </section>
  );
}


