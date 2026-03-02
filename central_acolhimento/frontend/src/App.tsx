import React, { useState } from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import Login from './shared/components/Login';
import WebApp from './apps/web/WebApp';
import MobileApp from './apps/mobile/MobileApp';
import { User } from './shared/types';

export default function App() {
  const [user, setUser] = useState<User | null>(null);

  const handleLogin = (role: 'COORDINATOR' | 'VOLUNTEER') => {
    setUser({
      id: '1',
      name: role === 'COORDINATOR' ? 'Carlos Eduardo' : 'Tiago Mendes',
      email: role === 'COORDINATOR' ? 'carlos@torre.com' : 'tiago@acolhimento.com',
      role,
      avatar: 'https://picsum.photos/seed/user/200'
    });
  };

  const handleLogout = () => setUser(null);

  return (
    <Router>
      <div className="min-h-screen bg-slate-50">
        <Routes>
          {!user ? (
            <Route path="*" element={<Login onLogin={handleLogin} />} />
          ) : (
            <>
              {user.role === 'COORDINATOR' ? (
                <Route path="*" element={<WebApp user={user} onLogout={handleLogout} />} />
              ) : (
                <Route path="*" element={<MobileApp user={user} onLogout={handleLogout} />} />
              )}
            </>
          )}
        </Routes>
      </div>
    </Router>
  );
}
