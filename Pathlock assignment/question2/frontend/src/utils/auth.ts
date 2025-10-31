const TOKEN_KEY = 'pl_token';
export function getToken(): string | null { return localStorage.getItem(TOKEN_KEY); }
export function setToken(t: string) { localStorage.setItem(TOKEN_KEY, t); }
export function logout() { localStorage.removeItem(TOKEN_KEY); }

export const API_BASE = (import.meta as any).env?.VITE_API_BASE ?? 'http://localhost:5280';

export async function api<T>(path: string, init?: RequestInit): Promise<T> {
  const token = getToken();
  const res = await fetch(`${API_BASE}${path}`, {
    headers: {
      'Content-Type': 'application/json',
      ...(token ? { Authorization: `Bearer ${token}` } : {})
    },
    ...init
  });
  if (!res.ok) throw new Error(await res.text());
  return res.status === 204 ? (undefined as unknown as T) : res.json();
}


