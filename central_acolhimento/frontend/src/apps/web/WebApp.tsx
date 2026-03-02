import React, { useState } from 'react';
import { motion } from 'motion/react';
import { 
  LayoutDashboard, 
  UserPlus, 
  BarChart3, 
  Users, 
  Settings, 
  Search, 
  Bell, 
  MoreHorizontal, 
  ChevronRight, 
  TrendingUp, 
  Home, 
  AlertCircle,
  LogOut,
  Plus
} from 'lucide-react';
import { User, Person, Caregiver } from '../../shared/types';
import { cn } from '../../shared/utils/cn';

interface WebAppProps {
  user: User;
  onLogout: () => void;
}

const MOCK_PEOPLE: Person[] = [
  { id: '1', name: 'Ana Souza', status: 'Novo', temperature: 'Morno', lastActivity: '3 dias', sector: 'Centro', description: 'Pendente retorno' },
  { id: '2', name: 'Carlos Alberto', status: 'Novo', temperature: 'Quente', lastActivity: 'Hoje', sector: 'Bela Vista', description: 'Enviou interesse' },
  { id: '3', name: 'Marcos Lima', status: 'Crescendo', temperature: 'Quente', lastActivity: '5 dias', sector: 'Vila Nova', description: 'Aguardando feedback' },
  { id: '4', name: 'Julia Silva', status: 'Firme', temperature: 'Frio', lastActivity: '2 dias', sector: 'Jardim das Flores', description: 'Consolidação ativa' },
];

const MOCK_CAREGIVERS: Caregiver[] = [
  { id: '1', name: 'João Silva', role: 'Irmão Visitador Sênior', avatar: 'https://picsum.photos/seed/j1/100', activeVisits: 1, status: 'Livre', lastActivity: 'Há 2h' },
  { id: '2', name: 'Maria Oliveira', role: 'Apoio em Enfermagem', avatar: 'https://picsum.photos/seed/m1/100', activeVisits: 3, status: 'Atenção', lastActivity: 'Há 15min' },
  { id: '3', name: 'Pedro Albuquerque', role: 'Psicólogo Social', avatar: 'https://picsum.photos/seed/p1/100', activeVisits: 5, status: 'Crítico', lastActivity: 'Em curso' },
];

export default function WebApp({ user, onLogout }: WebAppProps) {
  const [activeTab, setActiveTab] = useState('dashboard');

  return (
    <div className="flex h-screen overflow-hidden bg-slate-50">
      {/* Sidebar */}
      <aside className="w-64 border-r border-slate-200 bg-white flex flex-col shrink-0">
        <div className="p-6 flex items-center gap-3">
          <div className="bg-blue-600 rounded-lg p-2 text-white">
            <LayoutDashboard className="w-5 h-5" />
          </div>
          <div>
            <h1 className="text-sm font-bold leading-tight">Torre de Controle</h1>
            <p className="text-[10px] text-slate-500 uppercase tracking-wider font-bold">Sistema de Acolhimento</p>
          </div>
        </div>

        <nav className="flex-1 px-4 space-y-1">
          {[
            { id: 'dashboard', icon: LayoutDashboard, label: 'Dashboard' },
            { id: 'acolhimento', icon: UserPlus, label: 'Acolhimento' },
            { id: 'relatorios', icon: BarChart3, label: 'Relatórios' },
            { id: 'lideres', icon: Users, label: 'Líderes' },
            { id: 'configuracoes', icon: Settings, label: 'Configurações' },
          ].map((item) => (
            <button
              key={item.id}
              onClick={() => setActiveTab(item.id)}
              className={cn(
                "w-full flex items-center gap-3 px-3 py-2 text-sm font-medium rounded-lg transition-colors",
                activeTab === item.id 
                  ? "bg-blue-50 text-blue-600" 
                  : "text-slate-600 hover:bg-slate-100"
              )}
            >
              <item.icon className={cn("w-4 h-4", activeTab === item.id && "fill-blue-600/10")} />
              {item.label}
            </button>
          ))}
        </nav>

        <div className="p-4 border-t border-slate-200">
          <div className="flex items-center gap-3 p-2 mb-2">
            <img src={user.avatar} alt={user.name} className="w-8 h-8 rounded-full border border-slate-200" />
            <div className="flex-1 min-w-0">
              <p className="text-xs font-bold truncate">{user.name}</p>
              <p className="text-[10px] text-slate-500 truncate">Coordenador</p>
            </div>
          </div>
          <button 
            onClick={onLogout}
            className="w-full flex items-center justify-center gap-2 text-slate-500 hover:text-red-600 py-2 text-xs font-bold transition-colors"
          >
            <LogOut className="w-3 h-3" />
            Sair do Sistema
          </button>
        </div>
      </aside>

      {/* Main Content */}
      <main className="flex-1 flex flex-col overflow-hidden">
        {/* Header */}
        <header className="h-16 border-b border-slate-200 bg-white flex items-center justify-between px-8 shrink-0">
          <div className="flex items-center gap-4 bg-slate-100 px-4 py-2 rounded-xl w-96">
            <Search className="w-4 h-4 text-slate-400" />
            <input 
              type="text" 
              placeholder="Pesquisar acolhido, líder ou relatório..." 
              className="bg-transparent border-none focus:ring-0 text-sm w-full"
            />
          </div>
          <div className="flex items-center gap-4">
            <button className="p-2 text-slate-500 hover:bg-slate-100 rounded-full relative">
              <Bell className="w-5 h-5" />
              <span className="absolute top-2 right-2 w-2 h-2 bg-red-500 rounded-full border-2 border-white"></span>
            </button>
            <div className="h-8 w-px bg-slate-200 mx-2"></div>
            <button className="bg-blue-600 text-white px-4 py-2 rounded-lg font-bold text-sm shadow-lg shadow-blue-600/20 hover:bg-blue-700 transition-all flex items-center gap-2">
              <Plus className="w-4 h-4" />
              Novo Registro
            </button>
          </div>
        </header>

        {/* Dashboard Content */}
        <div className="flex-1 overflow-y-auto p-8">
          <div className="max-w-7xl mx-auto space-y-8">
            {/* KPI Cards */}
            <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
              <div className="bg-white p-6 rounded-2xl border border-slate-200 shadow-sm">
                <div className="flex justify-between items-start">
                  <p className="text-slate-500 text-xs font-bold uppercase tracking-wider">Taxa de Retenção TCI</p>
                  <TrendingUp className="text-emerald-500 w-4 h-4" />
                </div>
                <h3 className="text-3xl font-black mt-2">87.4%</h3>
                <div className="w-full bg-slate-100 h-1.5 rounded-full mt-4 overflow-hidden">
                  <div className="bg-blue-600 h-full rounded-full" style={{ width: '87.4%' }}></div>
                </div>
              </div>
              <div className="bg-white p-6 rounded-2xl border border-slate-200 shadow-sm">
                <div className="flex justify-between items-start">
                  <p className="text-slate-500 text-xs font-bold uppercase tracking-wider">Casas Ativas</p>
                  <Home className="text-blue-500 w-4 h-4" />
                </div>
                <h3 className="text-3xl font-black mt-2">92%</h3>
                <div className="w-full bg-slate-100 h-1.5 rounded-full mt-4 overflow-hidden">
                  <div className="bg-emerald-500 h-full rounded-full" style={{ width: '92%' }}></div>
                </div>
              </div>
              <div className="bg-white p-6 rounded-2xl border border-red-100 shadow-sm border-l-4 border-l-red-500">
                <div className="flex justify-between items-start">
                  <p className="text-slate-500 text-xs font-bold uppercase tracking-wider">Alerta de Inatividade</p>
                  <AlertCircle className="text-red-500 w-4 h-4" />
                </div>
                <h3 className="text-3xl font-black mt-2 text-red-600">14</h3>
                <p className="text-[10px] text-slate-400 font-bold mt-2">Exige intervenção nas últimas 48h</p>
              </div>
            </div>

            <div className="flex flex-col xl:flex-row gap-8">
              {/* Kanban Journey */}
              <div className="flex-1 space-y-4">
                <div className="flex items-center justify-between">
                  <h2 className="text-xl font-bold flex items-center gap-2">
                    <span className="w-2 h-6 bg-blue-600 rounded-full"></span>
                    Jornada Espiritual
                  </h2>
                  <button className="text-blue-600 text-sm font-bold hover:underline">Ver Mapa Completo</button>
                </div>

                <div className="flex gap-6 overflow-x-auto pb-4 no-scrollbar">
                  {['Novo Contato', 'Primeira Visita', 'Em Acompanhamento', 'Integrado'].map((col) => (
                    <div key={col} className="w-72 shrink-0 flex flex-col gap-4">
                      <div className="flex items-center justify-between px-2">
                        <span className="text-[10px] font-black uppercase tracking-widest text-slate-400">{col}</span>
                        <MoreHorizontal className="w-4 h-4 text-slate-300" />
                      </div>
                      <div className="flex flex-col gap-3">
                        {MOCK_PEOPLE.filter(p => {
                          if (col === 'Novo Contato') return p.status === 'Novo';
                          if (col === 'Em Acompanhamento') return p.status === 'Crescendo';
                          if (col === 'Integrado') return p.status === 'Firme';
                          return false;
                        }).map((person) => (
                          <motion.div 
                            key={person.id}
                            whileHover={{ y: -2 }}
                            className="bg-white p-4 rounded-xl border border-slate-200 shadow-sm hover:border-blue-300 transition-all cursor-pointer group"
                          >
                            <div className="flex justify-between items-start mb-2">
                              <h4 className="font-bold text-sm group-hover:text-blue-600 transition-colors">{person.name}</h4>
                              <span className={cn(
                                "px-2 py-0.5 rounded text-[8px] font-bold uppercase",
                                person.temperature === 'Quente' ? "bg-red-50 text-red-600" : "bg-blue-50 text-blue-600"
                              )}>
                                {person.lastActivity}
                              </span>
                            </div>
                            <p className="text-[10px] text-slate-500 mb-3">{person.description}</p>
                            <div className="flex justify-between items-center">
                              <div className="flex -space-x-2">
                                <div className="w-6 h-6 rounded-full bg-slate-100 border-2 border-white flex items-center justify-center text-[8px] font-bold">
                                  {person.name.split(' ').map(n => n[0]).join('')}
                                </div>
                              </div>
                              <ChevronRight className="w-4 h-4 text-slate-300 group-hover:text-blue-600 transition-colors" />
                            </div>
                          </motion.div>
                        ))}
                      </div>
                    </div>
                  ))}
                </div>
              </div>

              {/* Capacity Sidebar */}
              <aside className="w-full xl:w-80 space-y-6">
                <div className="bg-white rounded-2xl border border-slate-200 shadow-sm overflow-hidden">
                  <div className="p-4 border-b border-slate-100 bg-slate-50/50">
                    <h3 className="font-bold text-sm">Gestão de Capacidade</h3>
                    <p className="text-[10px] text-slate-400 font-bold uppercase tracking-widest mt-1">Cuidadores Ativos</p>
                  </div>
                  <div className="p-4 space-y-4">
                    {MOCK_CAREGIVERS.map((cg) => (
                      <div key={cg.id} className="flex items-center justify-between p-2 rounded-lg hover:bg-slate-50 transition-colors">
                        <div className="flex items-center gap-3">
                          <div className="relative">
                            <img src={cg.avatar} alt={cg.name} className="w-8 h-8 rounded-full" />
                            <span className={cn(
                              "absolute -bottom-0.5 -right-0.5 w-2.5 h-2.5 rounded-full border-2 border-white",
                              cg.status === 'Livre' ? "bg-emerald-500" : cg.status === 'Atenção' ? "bg-amber-500" : "bg-red-500"
                            )}></span>
                          </div>
                          <div className="flex flex-col">
                            <span className="text-xs font-bold">{cg.name}</span>
                            <span className="text-[10px] text-slate-400">{cg.activeVisits} acolhidos</span>
                          </div>
                        </div>
                        <span className={cn(
                          "text-[10px] font-bold px-2 py-1 rounded",
                          cg.status === 'Livre' ? "bg-emerald-50 text-emerald-600" : cg.status === 'Atenção' ? "bg-amber-50 text-amber-600" : "bg-red-50 text-red-600"
                        )}>
                          {cg.activeVisits}
                        </span>
                      </div>
                    ))}
                  </div>
                  <div className="p-4 bg-blue-50/50 border-t border-slate-100">
                    <div className="flex justify-between text-[10px] font-bold uppercase text-slate-400 mb-2">
                      <span>Capacidade Total</span>
                      <span className="text-blue-600">78%</span>
                    </div>
                    <div className="w-full bg-slate-200 h-1.5 rounded-full overflow-hidden">
                      <div className="bg-blue-600 h-full rounded-full" style={{ width: '78%' }}></div>
                    </div>
                  </div>
                </div>
              </aside>
            </div>
          </div>
        </div>
      </main>
    </div>
  );
}
