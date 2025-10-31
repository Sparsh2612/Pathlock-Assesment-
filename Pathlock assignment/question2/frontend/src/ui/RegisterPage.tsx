import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { api, setToken } from '../utils/auth';

export default function RegisterPage() {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState<string | null>(null);
  const nav = useNavigate();

  async function submit() {
    setError(null);
    try {
      const res = await api<{ token: string; username: string }>(`/api/auth/register`, {
        method: 'POST',
        body: JSON.stringify({ username, password })
      });
      setToken(res.token);
      nav('/', { replace: true });
    } catch (e: any) {
      setError(e.message ?? 'Registration failed');
    }
  }

  return (
    <section className="card">
      <h2>Register</h2>
      <div className="row"><input placeholder="Username" value={username} onChange={e=>setUsername(e.target.value)} /></div>
      <div className="row" style={{marginTop:8}}><input type="password" placeholder="Password" value={password} onChange={e=>setPassword(e.target.value)} /></div>
      {error && <p className="muted" style={{marginTop:8}}>{error}</p>}
      <div className="row" style={{marginTop:12}}>
        <button className="primary" onClick={submit}>Create account</button>
        <Link to="/login" className="muted" style={{alignSelf:'center'}}>Have an account? Login</Link>
      </div>
    </section>
  );
}


