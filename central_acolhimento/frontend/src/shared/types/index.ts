export type SpiritualStatus = 'Novo' | 'Crescendo' | 'Firme';
export type Temperature = 'Quente' | 'Morno' | 'Frio';

export interface Person {
  id: string;
  name: string;
  avatar?: string;
  status: SpiritualStatus;
  temperature: Temperature;
  lastActivity: string;
  sector: string;
  assignedTo?: string;
  description?: string;
}

export interface Caregiver {
  id: string;
  name: string;
  role: string;
  avatar: string;
  activeVisits: number;
  status: 'Livre' | 'Atenção' | 'Crítico';
  lastActivity: string;
}

export interface User {
  id: string;
  name: string;
  email: string;
  role: 'COORDINATOR' | 'VOLUNTEER';
  avatar: string;
}
