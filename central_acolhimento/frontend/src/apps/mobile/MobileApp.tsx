import React, { useState } from 'react';
import { motion, AnimatePresence } from 'motion/react';
import { 
  Home, 
  Users, 
  Clock, 
  UserCircle, 
  Bell, 
  Plus, 
  MessageSquare, 
  CheckCircle2, 
  ChevronRight, 
  Calendar,
  ArrowLeft,
  Send,
  AlertTriangle,
  Search,
  Phone,
  MapPin,
  Building2,
  User as UserIcon,
  Snowflake,
  Sun,
  Flame,
  FileText,
  UserPlus,
  MoreVertical,
  CalendarPlus,
  Users2
} from 'lucide-react';
import { User as UserType, Person } from '../../shared/types';
import { cn } from '../../shared/utils/cn';

interface MobileAppProps {
  user: UserType;
  onLogout: () => void;
}

const MOCK_MY_CARE: Person[] = [
  { id: '1', name: 'Maria Silva', status: 'Novo', temperature: 'Quente', lastActivity: 'Ontem', sector: 'Centro' },
  { id: '2', name: 'João Santos', status: 'Crescendo', temperature: 'Morno', lastActivity: '2 dias', sector: 'Vila Nova' },
  { id: '3', name: 'Ricardo Lima', status: 'Firme', temperature: 'Frio', lastActivity: '5 dias', sector: 'Jardim das Flores' },
];

export default function MobileApp({ user, onLogout }: MobileAppProps) {
  const [activeTab, setActiveTab] = useState('home');
  const [view, setView] = useState<'dashboard' | 'register-visit' | 'new-guest' | 'acolhidos'>('dashboard');
  const [acolhidosFilter, setAcolhidosFilter] = useState('Todos');
  const [searchTerm, setSearchTerm] = useState('');

  const renderDashboard = () => (
    <motion.div 
      initial={{ opacity: 0, y: 10 }}
      animate={{ opacity: 1, y: 0 }}
      className="flex-1 px-6 py-6 space-y-8 overflow-y-auto no-scrollbar"
    >
      {/* Header */}
      <header className="flex items-center justify-between">
        <div className="flex items-center gap-3">
          <div className="size-12 rounded-full border-2 border-blue-600/20 p-0.5 overflow-hidden">
            <img src={user.avatar} alt={user.name} className="w-full h-full object-cover rounded-full" />
          </div>
          <div>
            <h1 className="text-xl font-bold tracking-tight">Bom dia, {user.name.split(' ')[0]}</h1>
            <div className="flex items-center gap-1.5 mt-0.5">
              <span className="relative flex h-2 w-2">
                <span className="animate-ping absolute inline-flex h-full w-full rounded-full bg-green-400 opacity-75"></span>
                <span className="relative inline-flex rounded-full h-2 w-2 bg-green-500"></span>
              </span>
              <span className="text-[10px] font-bold text-slate-500 uppercase tracking-widest">WhatsApp Conectado</span>
            </div>
          </div>
        </div>
        <button className="p-2 text-slate-400 hover:text-blue-600 transition-colors">
          <Bell className="w-6 h-6" />
        </button>
      </header>

      {/* Quick Actions */}
      <div className="grid grid-cols-2 gap-4">
        <button 
          onClick={() => setView('register-visit')}
          className="flex flex-col items-center justify-center p-5 bg-blue-600 text-white rounded-2xl shadow-xl shadow-blue-600/20 hover:bg-blue-700 transition-all active:scale-95"
        >
          <Clock className="w-8 h-8 mb-2" />
          <span className="text-sm font-bold">Registrar Visita</span>
        </button>
        <button 
          onClick={() => setView('new-guest')}
          className="flex flex-col items-center justify-center p-5 bg-white border border-slate-200 text-slate-700 rounded-2xl hover:bg-slate-50 transition-all active:scale-95"
        >
          <Plus className="w-8 h-8 mb-2 text-blue-600" />
          <span className="text-sm font-bold">Novos Convidados</span>
        </button>
      </div>

      {/* Section: Meus Cuidados */}
      <section>
        <div className="flex items-center justify-between mb-4">
          <h2 className="text-lg font-bold text-slate-800">Meus Cuidados</h2>
          <button className="text-sm font-bold text-blue-600">Ver todos</button>
        </div>
        <div className="bg-white rounded-2xl p-5 border border-slate-100 shadow-sm space-y-4">
          <p className="text-xs text-slate-500 font-medium">3 pessoas sob sua responsabilidade direta esta semana.</p>
          <div className="flex items-center -space-x-3">
            {MOCK_MY_CARE.map((p, i) => (
              <div key={p.id} className="size-10 rounded-full ring-2 ring-white bg-slate-100 flex items-center justify-center text-[10px] font-bold">
                {p.name.split(' ').map(n => n[0]).join('')}
              </div>
            ))}
            <div className="flex size-10 items-center justify-center rounded-full border-2 border-dashed border-slate-300 bg-slate-50 text-slate-400">
              <Plus className="w-4 h-4" />
            </div>
          </div>
        </div>
      </section>

      {/* Section: Próxima Oração */}
      <section>
        <h2 className="text-lg font-bold text-slate-800 mb-4">Próxima Oração</h2>
        <div className="bg-white rounded-2xl p-5 border border-slate-100 shadow-sm flex items-center gap-4">
          <div className="flex size-14 items-center justify-center rounded-2xl bg-blue-50 text-blue-600">
            <Clock className="w-8 h-8 fill-blue-600/10" />
          </div>
          <div className="flex-1">
            <div className="flex justify-between items-start">
              <p className="text-2xl font-black tracking-tight text-slate-900">15:30</p>
              <span className="px-2 py-1 bg-blue-50 text-blue-600 text-[8px] font-black uppercase rounded">Hoje</span>
            </div>
            <p className="text-xs font-bold text-slate-500 uppercase tracking-widest mt-1">Sala de Oração - Ala Sul</p>
          </div>
        </div>
      </section>

      {/* Recent Messages */}
      <section className="pb-24">
        <h2 className="text-lg font-bold text-slate-800 mb-4">Últimas Mensagens</h2>
        <div className="space-y-3">
          <div className="flex gap-4 p-4 bg-white rounded-2xl border border-slate-100 shadow-sm">
            <div className="size-10 rounded-full bg-slate-100 flex items-center justify-center text-slate-500">
              <MessageSquare className="w-5 h-5" />
            </div>
            <div className="flex-1 min-w-0">
              <p className="text-sm font-bold">Maria Silva</p>
              <p className="text-xs text-slate-500 truncate">"Obrigado pelo acolhimento de ontem..."</p>
            </div>
            <span className="text-[10px] text-slate-400 font-bold">14:05</span>
          </div>
        </div>
      </section>
    </motion.div>
  );

  const renderRegisterVisit = () => (
    <motion.div 
      initial={{ opacity: 0, x: 20 }}
      animate={{ opacity: 1, x: 0 }}
      className="flex-1 flex flex-col bg-white"
    >
      <header className="px-6 py-6 border-b border-slate-100 flex items-center gap-4">
        <button onClick={() => setView('dashboard')} className="p-2 hover:bg-slate-100 rounded-full">
          <ArrowLeft className="w-6 h-6" />
        </button>
        <h2 className="text-xl font-bold">Registrar Visita</h2>
      </header>
      <div className="flex-1 p-6 space-y-6 overflow-y-auto">
        <div className="space-y-2">
          <label className="text-xs font-black uppercase tracking-widest text-slate-400">Pessoa Visitada</label>
          <div className="relative">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-400 w-5 h-5" />
            <input 
              type="text" 
              placeholder="Pesquisar pessoa ou grupo" 
              className="w-full pl-10 pr-4 py-4 bg-slate-50 border-none rounded-2xl text-sm focus:ring-2 focus:ring-blue-600/20"
            />
          </div>
        </div>

        <div className="space-y-2">
          <label className="text-xs font-black uppercase tracking-widest text-slate-400">Relatório da Visita</label>
          <textarea 
            rows={6}
            placeholder="Relate como foi a conversa, o que sentiu e os pontos principais abordados..."
            className="w-full p-4 bg-slate-50 border-none rounded-2xl text-sm focus:ring-2 focus:ring-blue-600/20 resize-none"
          ></textarea>
        </div>

        <div className="space-y-3">
          <label className="text-xs font-black uppercase tracking-widest text-slate-400">Estado Espiritual</label>
          <div className="grid grid-cols-3 gap-3">
            {['🌱 Novo', '🌿 Crescendo', '🌳 Firme'].map((status) => (
              <button key={status} className="flex flex-col items-center justify-center p-4 rounded-2xl border-2 border-slate-100 bg-white hover:border-blue-600 transition-all">
                <span className="text-sm font-bold">{status}</span>
              </button>
            ))}
          </div>
        </div>

        <div className="flex items-center justify-between p-4 bg-red-50 rounded-2xl border border-red-100">
          <div className="flex items-center gap-3">
            <AlertTriangle className="w-5 h-5 text-red-600" />
            <span className="text-sm font-bold text-red-700">Ajuda Urgente?</span>
          </div>
          <input type="checkbox" className="w-6 h-6 text-red-600 rounded-full border-red-200" />
        </div>
      </div>
      <div className="p-6 border-t border-slate-100">
        <button 
          onClick={() => setView('dashboard')}
          className="w-full py-4 bg-blue-600 text-white rounded-2xl font-bold shadow-xl shadow-blue-600/20 flex items-center justify-center gap-2"
        >
          <Send className="w-4 h-4" />
          Finalizar e Notificar Central
        </button>
      </div>
    </motion.div>
  );

  const renderNewGuest = () => (
    <motion.div 
      initial={{ opacity: 0, x: 20 }}
      animate={{ opacity: 1, x: 0 }}
      className="flex-1 flex flex-col bg-white"
    >
      <header className="px-6 py-6 border-b border-slate-100 flex items-center gap-4">
        <button onClick={() => setView('dashboard')} className="p-2 hover:bg-slate-100 rounded-full">
          <ArrowLeft className="w-6 h-6" />
        </button>
        <h2 className="text-xl font-bold">Novo Convidado</h2>
      </header>

      <div className="flex-1 p-6 space-y-6 overflow-y-auto no-scrollbar">
        <div className="bg-blue-50 p-4 rounded-2xl flex items-center gap-4">
          <div className="bg-blue-600 text-white p-2 rounded-xl">
            <UserPlus className="w-6 h-6" />
          </div>
          <div>
            <h3 className="font-bold text-slate-900">Boas-vindas!</h3>
            <p className="text-xs text-slate-500 font-medium">Registre um novo amigo hoje.</p>
          </div>
        </div>

        <div className="space-y-4">
          <h3 className="text-[10px] font-black uppercase tracking-widest text-slate-400">Informações Pessoais</h3>
          
          <div className="space-y-1.5">
            <label className="text-xs font-bold text-slate-700 ml-1">Nome Completo</label>
            <div className="relative">
              <UserIcon className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-400 w-5 h-5" />
              <input 
                type="text" 
                placeholder="Ex: João Silva" 
                className="w-full pl-10 pr-4 py-4 bg-slate-50 border-none rounded-2xl text-sm focus:ring-2 focus:ring-blue-600/20"
              />
            </div>
          </div>

          <div className="space-y-1.5">
            <label className="text-xs font-bold text-slate-700 ml-1">WhatsApp</label>
            <div className="relative">
              <Phone className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-400 w-5 h-5" />
              <input 
                type="tel" 
                placeholder="(00) 00000-0000" 
                className="w-full pl-10 pr-4 py-4 bg-slate-50 border-none rounded-2xl text-sm focus:ring-2 focus:ring-blue-600/20"
              />
            </div>
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div className="space-y-1.5">
              <label className="text-xs font-bold text-slate-700 ml-1">Bairro</label>
              <div className="relative">
                <MapPin className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-400 w-5 h-5" />
                <input 
                  type="text" 
                  placeholder="Onde mora?" 
                  className="w-full pl-10 pr-4 py-4 bg-slate-50 border-none rounded-2xl text-sm focus:ring-2 focus:ring-blue-600/20"
                />
              </div>
            </div>
            <div className="space-y-1.5">
              <label className="text-xs font-bold text-slate-700 ml-1">Cidade</label>
              <div className="relative">
                <Building2 className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-400 w-5 h-5" />
                <input 
                  type="text" 
                  placeholder="Sua cidade" 
                  className="w-full pl-10 pr-4 py-4 bg-slate-50 border-none rounded-2xl text-sm focus:ring-2 focus:ring-blue-600/20"
                />
              </div>
            </div>
          </div>

          <div className="space-y-1.5">
            <label className="text-xs font-bold text-slate-700 ml-1">Quem Convidou?</label>
            <div className="relative">
              <Users className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-400 w-5 h-5" />
              <input 
                type="text" 
                placeholder="Nome de quem convidou" 
                className="w-full pl-10 pr-4 py-4 bg-slate-50 border-none rounded-2xl text-sm focus:ring-2 focus:ring-blue-600/20"
              />
            </div>
          </div>

          <div className="space-y-2 pt-2">
            <label className="text-xs font-bold text-slate-700 ml-1">Interesse / Engajamento</label>
            <div className="grid grid-cols-3 gap-2 bg-slate-100 p-1 rounded-2xl">
              <button className="flex flex-col items-center justify-center py-3 rounded-xl bg-white shadow-sm border-2 border-transparent">
                <Snowflake className="w-5 h-5 text-blue-500 mb-1" />
                <span className="text-[10px] font-bold text-slate-600">Frio</span>
              </button>
              <button className="flex flex-col items-center justify-center py-3 rounded-xl border-2 border-transparent">
                <Sun className="w-5 h-5 text-orange-400 mb-1" />
                <span className="text-[10px] font-bold text-slate-600">Morno</span>
              </button>
              <button className="flex flex-col items-center justify-center py-3 rounded-xl border-2 border-transparent">
                <Flame className="w-5 h-5 text-red-500 mb-1" />
                <span className="text-[10px] font-bold text-slate-600">Quente</span>
              </button>
            </div>
          </div>

          <div className="space-y-1.5">
            <label className="text-xs font-bold text-slate-700 ml-1">Observações Iniciais</label>
            <div className="relative">
              <FileText className="absolute left-3 top-4 text-slate-400 w-5 h-5" />
              <textarea 
                rows={3}
                placeholder="Conte-nos um pouco sobre a primeira conversa..."
                className="w-full pl-10 pr-4 py-4 bg-slate-50 border-none rounded-2xl text-sm focus:ring-2 focus:ring-blue-600/20 resize-none"
              ></textarea>
            </div>
          </div>
        </div>
      </div>
      <div className="p-6 border-t border-slate-100">
        <button 
          onClick={() => setView('dashboard')}
          className="w-full py-4 bg-blue-600 text-white rounded-2xl font-bold shadow-xl shadow-blue-600/20 flex items-center justify-center gap-2 transition-all active:scale-95"
        >
          <UserPlus className="w-4 h-4" />
          Cadastrar e Iniciar Jornada
        </button>
      </div>
    </motion.div>
  );

  const renderAcolhidos = () => (
    <motion.div 
      initial={{ opacity: 0, x: 20 }}
      animate={{ opacity: 1, x: 0 }}
      className="flex-1 flex flex-col bg-slate-50"
    >
      <header className="flex items-center bg-white px-4 py-4 sticky top-0 z-10 border-b border-slate-200">
        <button onClick={() => { setView('dashboard'); setActiveTab('home'); }} className="flex items-center justify-center p-2 rounded-full hover:bg-slate-100 text-slate-700">
          <ArrowLeft className="w-6 h-6" />
        </button>
        <h1 className="text-xl font-bold leading-tight tracking-tight flex-1 ml-2 text-slate-900">Acolhidos</h1>
        <button className="flex items-center justify-center p-2 rounded-full hover:bg-slate-100 text-slate-700">
          <MoreVertical className="w-6 h-6" />
        </button>
      </header>

      <div className="px-4 py-4 bg-white">
        <div className="flex w-full items-stretch rounded-xl h-12 border border-slate-200 bg-slate-50 focus-within:border-blue-600 focus-within:ring-1 focus-within:ring-blue-600 transition-all">
          <div className="flex items-center justify-center pl-4 text-slate-400">
            <Search className="w-5 h-5" />
          </div>
          <input 
            className="w-full min-w-0 flex-1 border-none bg-transparent focus:ring-0 text-slate-900 placeholder:text-slate-400 px-3 text-base" 
            placeholder="Buscar acolhido por nome..." 
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
          />
        </div>
      </div>

      <div className="bg-white sticky top-[73px] z-10">
        <div className="flex overflow-x-auto no-scrollbar border-b border-slate-200 px-4 gap-6">
          {['Todos', '🌱 Novo', '🌿 Crescendo', '🌳 Firme'].map((tab) => (
            <button 
              key={tab}
              onClick={() => setAcolhidosFilter(tab)}
              className={cn(
                "flex flex-col items-center justify-center border-b-2 pb-3 pt-2 shrink-0 transition-all",
                acolhidosFilter === tab ? "border-blue-600 text-blue-600" : "border-transparent text-slate-500"
              )}
            >
              <p className={cn("text-sm tracking-wide", acolhidosFilter === tab ? "font-bold" : "font-medium")}>{tab}</p>
            </button>
          ))}
        </div>
      </div>

      <main className="flex-1 p-4 space-y-3 overflow-y-auto no-scrollbar pb-24">
        {[
          { id: '1', name: 'João Silva', sector: 'Centro', status: '🌱 Novo', avatar: 'https://picsum.photos/seed/joao/200' },
          { id: '2', name: 'Maria Oliveira', sector: 'Vila Nova', status: '🌿 Crescendo', avatar: 'https://picsum.photos/seed/maria/200' },
          { id: '3', name: 'Pedro Santos', sector: 'Jardim das Flores', status: '🌳 Firme', avatar: 'https://picsum.photos/seed/pedro/200' },
          { id: '4', name: 'Ana Luiza', sector: 'Bela Vista', status: '🌱 Novo', avatar: 'https://picsum.photos/seed/ana/200' },
        ].filter(p => {
          const matchesFilter = acolhidosFilter === 'Todos' || p.status === acolhidosFilter;
          const matchesSearch = p.name.toLowerCase().includes(searchTerm.toLowerCase());
          return matchesFilter && matchesSearch;
        }).map((person) => (
          <div key={person.id} className="bg-white rounded-xl p-4 flex items-center gap-4 shadow-sm border border-slate-100">
            <img 
              src={person.avatar} 
              alt={person.name}
              className="aspect-square rounded-full h-14 w-14 shrink-0 object-cover"
              referrerPolicy="no-referrer"
            />
            <div className="flex flex-col flex-1">
              <p className="text-slate-900 text-base font-bold leading-tight">{person.name}</p>
              <p className="text-slate-500 text-[10px] font-bold mt-1 uppercase tracking-wider">{person.sector}</p>
              <div className="mt-2">
                <span className="inline-flex items-center px-2 py-0.5 rounded-full text-[10px] font-bold bg-emerald-50 text-emerald-600 border border-emerald-100">
                  {person.status}
                </span>
              </div>
            </div>
            <div className="shrink-0">
              <button 
                onClick={() => setView('register-visit')}
                className="bg-blue-50 hover:bg-blue-100 text-blue-600 p-2 rounded-lg transition-colors flex items-center gap-2"
              >
                <CalendarPlus className="w-5 h-5" />
                <span className="text-[10px] font-bold">Visita</span>
              </button>
            </div>
          </div>
        ))}
      </main>

      <button 
        onClick={() => setView('new-guest')}
        className="fixed bottom-24 right-6 bg-blue-600 text-white size-14 rounded-full shadow-lg flex items-center justify-center hover:scale-105 transition-transform active:scale-95 z-20"
      >
        <UserPlus className="w-6 h-6" />
      </button>
    </motion.div>
  );

  return (
    <div className="min-h-screen bg-slate-50 flex flex-col max-w-md mx-auto shadow-2xl relative overflow-hidden font-display">
      <AnimatePresence mode="wait">
        {view === 'dashboard' && renderDashboard()}
        {view === 'register-visit' && renderRegisterVisit()}
        {view === 'new-guest' && renderNewGuest()}
        {view === 'acolhidos' && renderAcolhidos()}
      </AnimatePresence>

      {/* Bottom Nav */}
      {(view === 'dashboard' || view === 'acolhidos') && (
        <nav className="fixed bottom-0 left-0 right-0 max-w-md mx-auto bg-white/80 backdrop-blur-md border-t border-slate-100 px-6 py-3 flex justify-between items-center z-50">
          {[
            { id: 'home', icon: Home, label: 'Início', view: 'dashboard' },
            { id: 'acolhidos', icon: Users, label: 'Acolhidos', view: 'acolhidos' },
            { id: 'eventos', icon: Calendar, label: 'Eventos', view: 'acolhidos' },
            { id: 'grupos', icon: Users2, label: 'Grupos', view: 'acolhidos' },
            { id: 'perfil', icon: UserCircle, label: 'Perfil', view: 'acolhidos' },
          ].map((item) => (
            <button
              key={item.id}
              onClick={() => {
                setActiveTab(item.id);
                setView(item.view as any);
              }}
              className={cn(
                "flex flex-col items-center gap-1 p-2 transition-colors",
                activeTab === item.id ? "text-blue-600" : "text-slate-400"
              )}
            >
              <item.icon className={cn("w-6 h-6", activeTab === item.id && "fill-blue-600/10")} />
              <span className="text-[10px] font-bold">{item.label}</span>
            </button>
          ))}
        </nav>
      )}
    </div>
  );
}
