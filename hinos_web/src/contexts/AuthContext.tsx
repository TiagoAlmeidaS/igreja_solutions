import React, { createContext, useContext, useState, ReactNode, useEffect } from 'react';
import { login as apiLogin, logout as apiLogout, LoginResponse } from '../services/api';

interface User {
  id: string;
  name: string;
  email: string;
}

interface AuthContextType {
  user: User | null;
  login: (email: string, password: string) => Promise<boolean>;
  logout: () => void;
  isAuthenticated: boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<User | null>(null);

  useEffect(() => {
    const savedUser = localStorage.getItem('user');
    const savedToken = localStorage.getItem('auth_token');
    
    if (savedUser && savedToken) {
      try {
        setUser(JSON.parse(savedUser));
      } catch (error) {
        console.error('Erro ao recuperar usuário do localStorage:', error);
        localStorage.removeItem('user');
        localStorage.removeItem('auth_token');
      }
    }
  }, []);

  const login = async (email: string, password: string): Promise<boolean> => {
    try {
      const response: LoginResponse = await apiLogin({ email, password });
      
      const userData: User = {
        id: response.user.id,
        name: response.user.name,
        email: response.user.email,
      };
      
      setUser(userData);
      localStorage.setItem('user', JSON.stringify(userData));
      
      return true;
    } catch (error: any) {
      console.error('Erro ao fazer login:', error);
      return false;
    }
  };

  const logout = () => {
    setUser(null);
    localStorage.removeItem('user');
    apiLogout();
  };

  return (
    <AuthContext.Provider value={{ 
      user, 
      login, 
      logout, 
      isAuthenticated: !!user 
    }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within AuthProvider');
  }
  return context;
}
