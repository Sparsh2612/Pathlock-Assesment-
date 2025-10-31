import React from 'react';
import { Link, Navigate, Route, Routes, useLocation } from 'react-router-dom';
import LoginPage from './LoginPage';
import RegisterPage from './RegisterPage';
import DashboardPage from './DashboardPage';
import ProjectPage from './ProjectPage';
import { getToken, logout } from '../utils/auth';

function Private({ children }: { children: React.ReactNode }) {
  const authed = !!getToken();
  const location = useLocation();
  return authed ? <>{children}</> : <Navigate to="/login" replace state={{ from: location }} />;
}

export default function App() {
  const authed = !!getToken();
  return (
    <div className="app">
      <header className="header">
        <div className="brand">
          <img src="/src/assets/pathlock.png" alt="Pathlock" />
          <h1>Pathlock Projects</h1>
        </div>
        <nav className="nav">
          {authed ? (
            <>
              <Link to="/">Dashboard</Link>
              <button className="ghost" onClick={() => { logout(); location.href = '/login'; }}>Logout</button>
            </>
          ) : (
            <>
              <Link to="/login">Login</Link>
              <Link to="/register">Register</Link>
            </>
          )}
        </nav>
      </header>

      <main className="main">
        <Routes>
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />
          <Route path="/" element={<Private><DashboardPage /></Private>} />
          <Route path="/projects/:id" element={<Private><ProjectPage /></Private>} />
        </Routes>
      </main>
    </div>
  );
}


