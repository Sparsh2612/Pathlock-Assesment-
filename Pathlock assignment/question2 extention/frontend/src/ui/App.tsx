import React, { useMemo, useState } from 'react';

type TaskInput = { title: string; estimatedHours: number; dueDate?: string; dependencies: string[] };
const API_BASE = (import.meta as any).env?.VITE_API_BASE ?? 'http://localhost:5380';
const PROJECT_ID = 1;

async function schedule(tasks: TaskInput[]) {
  const res = await fetch(`${API_BASE}/api/v1/projects/${PROJECT_ID}/schedule`, {
    method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ tasks })
  });
  if (!res.ok) throw new Error(await res.text());
  return res.json() as Promise<{ recommendedOrder: string[] }>;
}

export default function App() {
  const [tasks, setTasks] = useState<TaskInput[]>([
    { title: 'Design API', estimatedHours: 5, dueDate: '2025-10-25', dependencies: [] },
    { title: 'Implement Backend', estimatedHours: 12, dueDate: '2025-10-28', dependencies: ['Design API'] },
    { title: 'Build Frontend', estimatedHours: 10, dueDate: '2025-10-30', dependencies: ['Design API'] },
    { title: 'End-to-End Test', estimatedHours: 8, dueDate: '2025-10-31', dependencies: ['Implement Backend', 'Build Frontend'] }
  ]);
  const [newTitle, setNewTitle] = useState('');
  const [newHours, setNewHours] = useState(4);
  const [newDue, setNewDue] = useState('');
  const [deps, setDeps] = useState<string[]>([]);
  const [order, setOrder] = useState<string[] | null>(null);
  const [error, setError] = useState<string | null>(null);

  const availableDeps = useMemo(() => tasks.map(t => t.title).filter(t => t !== newTitle), [tasks, newTitle]);

  function addTask() {
    const title = newTitle.trim(); if (!title) return;
    setTasks(prev => [...prev, { title, estimatedHours: newHours, dueDate: newDue || undefined, dependencies: deps }]);
    setNewTitle(''); setNewHours(4); setNewDue(''); setDeps([]);
  }

  function toggleDep(title: string) {
    setDeps(d => d.includes(title) ? d.filter(x => x !== title) : [...d, title]);
  }

  async function run() {
    try { setError(null); const res = await schedule(tasks); setOrder(res.recommendedOrder); } catch (e:any) { setError(e.message); }
  }

  return (
    <div className="app">
      <header className="header">
        <div className="brand">
          <img src="/src/assets/pathlock.png" alt="Pathlock" />
          <h1>Smart Scheduler</h1>
        </div>
      </header>

      <main className="main">
        <section className="card" style={{maxWidth:980}}>
          <h2>Define Tasks</h2>
          <div className="row">
            <input placeholder="Title" value={newTitle} onChange={e=>setNewTitle(e.target.value)} />
            <input type="number" min={1} max={1000} value={newHours} onChange={e=>setNewHours(parseInt(e.target.value||'1'))} style={{maxWidth:160}} />
            <input type="date" value={newDue} onChange={e=>setNewDue(e.target.value)} style={{maxWidth:200}} />
            <button className="primary" onClick={addTask}>Add</button>
          </div>
          <div className="row" style={{marginTop:6, flexWrap:'wrap', gap:8}}>
            {availableDeps.map(t => (
              <button key={t} className={deps.includes(t)?'chip active':'chip'} onClick={()=>toggleDep(t)}>{t}</button>
            ))}
          </div>

          <ul className="list">
            {tasks.map(t => (
              <li className="item" key={t.title}>
                <div style={{flex:1}}>
                  <strong>{t.title}</strong>
                  <div className="muted" style={{fontSize:12}}>Hours: {t.estimatedHours} · Due: {t.dueDate ?? '—'}</div>
                </div>
                <div className="muted" style={{fontSize:12}}>Deps: {t.dependencies.join(', ') || 'None'}</div>
              </li>
            ))}
          </ul>

          <div className="row" style={{marginTop:12}}>
            <button className="primary" onClick={run}>Generate Schedule</button>
            <span className="muted" style={{alignSelf:'center'}}>API: {API_BASE}</span>
          </div>
          {error && <p className="muted" style={{marginTop:8}}>{error}</p>}

          {order && (
            <div style={{marginTop:16}}>
              <h3>Recommended Order</h3>
              <ol>
                {order.map(o => <li key={o}>{o}</li>)}
              </ol>
            </div>
          )}
        </section>
      </main>
    </div>
  );
}


