export interface ChatMessage {
  id: string;
  role: 'user' | 'assistant';
  content: string;
  timestamp: Date;
}

export interface HymnRequest {
  id: string;
  title?: string;
  number?: string;
  category?: string;
  lyrics?: string;
  status: 'pending' | 'processing' | 'completed';
  createdAt: Date;
}
