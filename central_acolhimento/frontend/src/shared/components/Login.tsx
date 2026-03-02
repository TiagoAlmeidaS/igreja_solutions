import React, { useState } from 'react';
import { motion } from 'motion/react';
import { Castle, Mail, Lock, Eye, EyeOff } from 'lucide-react';

interface LoginProps {
  onLogin: (role: 'COORDINATOR' | 'VOLUNTEER') => void;
}

export default function Login({ onLogin }: LoginProps) {
  const [showPassword, setShowPassword] = useState(false);
  const [email, setEmail] = useState('');

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    // For demo, if email contains 'coord' -> coordinator, else volunteer
    if (email.toLowerCase().includes('coord')) {
      onLogin('COORDINATOR');
    } else {
      onLogin('VOLUNTEER');
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center p-4 bg-slate-50">
      <motion.div 
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        className="w-full max-w-md"
      >
        <div className="flex flex-col items-center mb-8">
          <div className="w-16 h-16 bg-blue-600/10 rounded-2xl flex items-center justify-center mb-4">
            <Castle className="text-blue-600 w-10 h-10" />
          </div>
          <h1 className="text-3xl font-bold tracking-tight text-slate-900">Torre de Controle</h1>
          <p className="text-slate-500 font-medium mt-1">Sistema de Acolhimento</p>
        </div>

        <div className="bg-white shadow-xl shadow-slate-200/50 rounded-2xl p-8 border border-slate-100">
          <form onSubmit={handleSubmit} className="space-y-6">
            <div className="space-y-2">
              <label className="text-sm font-semibold text-slate-700">E-mail</label>
              <div className="relative">
                <Mail className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-400 w-5 h-5" />
                <input 
                  type="email"
                  required
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  placeholder="seu@email.com"
                  className="block w-full pl-10 pr-3 py-3 border border-slate-200 rounded-xl bg-slate-50 focus:outline-none focus:ring-2 focus:ring-blue-600/20 focus:border-blue-600 transition-all"
                />
              </div>
            </div>

            <div className="space-y-2">
              <div className="flex justify-between items-center">
                <label className="text-sm font-semibold text-slate-700">Senha</label>
                <button type="button" className="text-sm font-medium text-blue-600 hover:underline">Esqueci minha senha</button>
              </div>
              <div className="relative">
                <Lock className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-400 w-5 h-5" />
                <input 
                  type={showPassword ? 'text' : 'password'}
                  required
                  placeholder="••••••••"
                  className="block w-full pl-10 pr-12 py-3 border border-slate-200 rounded-xl bg-slate-50 focus:outline-none focus:ring-2 focus:ring-blue-600/20 focus:border-blue-600 transition-all"
                />
                <button 
                  type="button"
                  onClick={() => setShowPassword(!showPassword)}
                  className="absolute right-3 top-1/2 -translate-y-1/2 text-slate-400 hover:text-slate-600"
                >
                  {showPassword ? <EyeOff className="w-5 h-5" /> : <Eye className="w-5 h-5" />}
                </button>
              </div>
            </div>

            <div className="flex items-center">
              <input type="checkbox" id="remember" className="h-4 w-4 text-blue-600 rounded border-slate-300" />
              <label htmlFor="remember" className="ml-2 text-sm text-slate-600 cursor-pointer">Lembrar de mim</label>
            </div>

            <button 
              type="submit"
              className="w-full py-4 px-4 bg-blue-600 text-white rounded-xl font-bold shadow-lg shadow-blue-600/20 hover:bg-blue-700 transition-all active:scale-[0.98]"
            >
              Entrar no Sistema
            </button>
          </form>
        </div>

        <div className="mt-10 text-center space-y-4">
          <p className="text-sm text-slate-500">A serviço do Reino em Sapé</p>
          <div className="flex justify-center gap-4 text-slate-400 text-xs uppercase tracking-widest font-bold">
            <button className="hover:text-blue-600">Suporte</button>
            <span>•</span>
            <button className="hover:text-blue-600">Privacidade</button>
            <span>•</span>
            <button className="hover:text-blue-600">Termos</button>
          </div>
        </div>
      </motion.div>
    </div>
  );
}
