import React, { useState } from 'react';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { api, setToken } from '../utils/auth';

export default function LoginPage() {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState<string | null>(null);
  const nav = useNavigate();
  const location = useLocation() as any;

  async function submit() {
    setError(null);
    try {
      const res = await api<{ token: string; username: string }>(`/api/auth/login`, {
        method: 'POST',
        body: JSON.stringify({ username, password })
      });
      setToken(res.token);
      const to = location.state?.from?.pathname ?? '/';
      nav(to, { replace: true });
    } catch (e: any) {
      setError(e.message ?? 'Login failed');
    }
  }

  return (
    <section className="card">
      <h2>Login</h2>
      <div className="row"><input placeholder="Username" value={username} onChange={e=>setUsername(e.target.value)} /></div>
      <div className="row" style={{marginTop:8}}><input type="password" placeholder="Password" value={password} onChange={e=>setPassword(e.target.value)} /></div>
      {error && <p className="muted" style={{marginTop:8}}>{error}</p>}
      <div className="row" style={{marginTop:12}}>
        <button className="primary" onClick={submit}>Login</button>
        <Link to="/register" className="muted" style={{alignSelf:'center'}}>Create account</Link>
      </div>
    </section>
  );
}


