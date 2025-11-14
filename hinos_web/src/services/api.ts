import { Hymn, Verse } from '../types/hymn';

// Se VITE_API_URL estiver vazio ou undefined, usa caminho relativo (para Caddy)
// Caso contrário, usa a URL configurada (para desenvolvimento local)
const API_URL = import.meta.env.VITE_API_URL ?? '';

export interface ApiError {
  message: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  user: {
    id: string;
    name: string;
    email: string;
  };
}

export interface CreateHymnRequest {
  number: string;
  title: string;
  category: 'hinario' | 'canticos' | 'suplementar' | 'novos';
  hymnBook: string;
  key?: string;
  bpm?: number;
  verses: Array<{
    type: 'V1' | 'V2' | 'V3' | 'V4' | 'R' | 'Ponte' | 'C';
    lines: string[];
  }>;
}

export interface UpdateHymnRequest extends CreateHymnRequest {}

interface ApiHymn {
  id: number;
  number: string;
  title: string;
  category: 'hinario' | 'canticos' | 'suplementar' | 'novos';
  hymnBook: string;
  verses: Array<{
    type: 'V1' | 'V2' | 'V3' | 'V4' | 'R' | 'Ponte' | 'C';
    lines: string[];
  }>;
  key?: string;
  bpm?: number;
}

async function handleResponse<T>(response: Response): Promise<T> {
  if (!response.ok) {
    let errorMessage = `Erro HTTP: ${response.status} ${response.statusText}`;
    
    try {
      const error: ApiError = await response.json();
      errorMessage = error.message || errorMessage;
    } catch {
      // Se não conseguir parsear JSON, usar mensagem padrão
    }
    
    // Tratamento específico para erros conhecidos
    if (response.status === 409) {
      throw new Error('Já existe um hino com este número');
    }
    if (response.status === 404) {
      throw new Error('Hino não encontrado');
    }
    if (response.status === 401) {
      throw new Error('Não autorizado. Faça login novamente.');
    }
    
    throw new Error(errorMessage);
  }
  
  // Para respostas 204 No Content, retornar void
  if (response.status === 204) {
    return undefined as T;
  }
  
  return response.json();
}

// Helper para obter token do localStorage
function getAuthToken(): string | null {
  return localStorage.getItem('auth_token');
}

// Helper para adicionar token de autenticação aos headers
function getAuthHeaders(): HeadersInit {
  const token = getAuthToken();
  const headers: HeadersInit = {
    'Content-Type': 'application/json',
  };
  
  if (token) {
    headers['Authorization'] = `Bearer ${token}`;
  }
  
  return headers;
}

// POST /api/auth/login - Login de autenticação
export async function login(credentials: LoginRequest): Promise<LoginResponse> {
  try {
    const url = API_URL ? `${API_URL}/api/auth/login` : '/api/auth/login';
    const response = await fetch(url, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(credentials),
    });
    
    const data = await handleResponse<LoginResponse>(response);
    
    // Armazenar token no localStorage
    localStorage.setItem('auth_token', data.token);
    
    return data;
  } catch (error) {
    console.error('Erro ao fazer login:', error);
    throw error;
  }
}

// Logout - Remove token do localStorage
export function logout(): void {
  localStorage.removeItem('auth_token');
}

function mapApiHymnToHymn(apiHymn: ApiHymn): Hymn {
  return {
    id: apiHymn.id.toString(),
    number: apiHymn.number,
    title: apiHymn.title,
    category: apiHymn.category,
    hymnBook: apiHymn.hymnBook,
    verses: apiHymn.verses,
    key: apiHymn.key,
    bpm: apiHymn.bpm,
  };
}

// GET /api/hymns - Lista todos os hinos (com filtros opcionais)
export async function getAllHymns(
  category?: string,
  search?: string
): Promise<Hymn[]> {
  try {
    const params = new URLSearchParams();
    if (category && category !== 'todos') params.append('category', category);
    if (search) params.append('search', search);

    const url = API_URL ? `${API_URL}/api/hymns${params.toString() ? `?${params.toString()}` : ''}` : `/api/hymns${params.toString() ? `?${params.toString()}` : ''}`;
    const response = await fetch(url);
    const apiHymns = await handleResponse<ApiHymn[]>(response);
    return apiHymns.map(mapApiHymnToHymn);
  } catch (error) {
    console.error('Erro ao buscar hinos:', error);
    throw error;
  }
}

// Helper para converter string ID para número (suporta IDs negativos do SQLite)
function parseHymnId(id: string): number {
  const parsed = parseInt(id, 10);
  if (isNaN(parsed)) {
    throw new Error(`ID inválido: ${id}`);
  }
  return parsed;
}

// GET /api/hymns/{id} - Busca hino por ID
export async function getHymnById(id: string): Promise<Hymn | null> {
  try {
    // Converter string para int (API espera int, incluindo negativos para SQLite)
    const hymnId = parseHymnId(id);
    const url = API_URL ? `${API_URL}/api/hymns/${hymnId}` : `/api/hymns/${hymnId}`;
    const response = await fetch(url);
    const apiHymn = await handleResponse<ApiHymn>(response);
    return mapApiHymnToHymn(apiHymn);
  } catch (error: any) {
    if (error.message.includes('404') || error.message.includes('não encontrado')) {
      return null;
    }
    console.error('Erro ao buscar hino:', error);
    throw error;
  }
}

// GET /api/hymns/search?term={term} - Busca por termo
export async function searchHymns(term: string): Promise<Hymn[]> {
  const trimmedTerm = term.trim();
  
  // Se termo está vazio, usar getAllHymns em vez do endpoint de busca
  // (o endpoint /api/hymns/search requer term obrigatório)
  if (!trimmedTerm) {
    return getAllHymns();
  }

  try {
    const params = new URLSearchParams({ term: trimmedTerm });
    const url = API_URL ? `${API_URL}/api/hymns/search?${params.toString()}` : `/api/hymns/search?${params.toString()}`;
    const response = await fetch(url);
    const apiHymns = await handleResponse<ApiHymn[]>(response);
    return apiHymns.map(mapApiHymnToHymn);
  } catch (error) {
    console.error('Erro ao buscar hinos:', error);
    throw error;
  }
}

// POST /api/hymns - Cria novo hino
export async function createHymn(hymn: CreateHymnRequest): Promise<Hymn> {
  try {
    const url = API_URL ? `${API_URL}/api/hymns` : '/api/hymns';
    const response = await fetch(url, {
      method: 'POST',
      headers: getAuthHeaders(),
      body: JSON.stringify(hymn),
    });
    
    const apiHymn = await handleResponse<ApiHymn>(response);
    return mapApiHymnToHymn(apiHymn);
  } catch (error) {
    console.error('Erro ao criar hino:', error);
    throw error;
  }
}

// PUT /api/hymns/{id} - Atualiza hino existente
export async function updateHymn(id: string, hymn: UpdateHymnRequest): Promise<Hymn> {
  try {
    // Verificar se é ID negativo (SQLite - somente leitura)
    const hymnId = parseHymnId(id);
    if (hymnId < 0) {
      throw new Error('Hinos do hinário base são somente leitura e não podem ser editados');
    }
    
    const url = API_URL ? `${API_URL}/api/hymns/${hymnId}` : `/api/hymns/${hymnId}`;
    const response = await fetch(url, {
      method: 'PUT',
      headers: getAuthHeaders(),
      body: JSON.stringify(hymn),
    });
    
    const apiHymn = await handleResponse<ApiHymn>(response);
    return mapApiHymnToHymn(apiHymn);
  } catch (error) {
    console.error('Erro ao atualizar hino:', error);
    throw error;
  }
}

// DELETE /api/hymns/{id} - Remove hino
export async function deleteHymn(id: string): Promise<void> {
  try {
    // Verificar se é ID negativo (SQLite - somente leitura)
    const hymnId = parseHymnId(id);
    if (hymnId < 0) {
      throw new Error('Hinos do hinário base são somente leitura e não podem ser removidos');
    }
    
    const url = API_URL ? `${API_URL}/api/hymns/${hymnId}` : `/api/hymns/${hymnId}`;
    const response = await fetch(url, {
      method: 'DELETE',
      headers: getAuthHeaders(),
    });
    
    await handleResponse<void>(response);
  } catch (error) {
    console.error('Erro ao deletar hino:', error);
    throw error;
  }
}

// Interface para retornar blob e nome do arquivo
export interface DownloadResult {
  blob: Blob;
  fileName: string;
}

// Helper para extrair nome do arquivo do header Content-Disposition
function extractFileNameFromHeaders(headers: Headers, defaultName: string): string {
  const contentDisposition = headers.get('Content-Disposition');
  if (contentDisposition) {
    const fileNameMatch = contentDisposition.match(/filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/);
    if (fileNameMatch && fileNameMatch[1]) {
      return fileNameMatch[1].replace(/['"]/g, '');
    }
  }
  return defaultName;
}

// GET /api/hymns/{id}/download/plain - Download hino em formato texto plano
export async function downloadHymnPlain(id: string): Promise<DownloadResult> {
  try {
    const hymnId = parseHymnId(id);
    const url = API_URL ? `${API_URL}/api/hymns/${hymnId}/download/plain` : `/api/hymns/${hymnId}/download/plain`;
    const response = await fetch(url, {
      headers: getAuthHeaders(),
    });
    
    if (!response.ok) {
      if (response.status === 404) {
        throw new Error('Hino não encontrado');
      }
      throw new Error(`Erro ao baixar hino: ${response.statusText}`);
    }
    
    const blob = await response.blob();
    const fileName = extractFileNameFromHeaders(response.headers, 'hino-plain.txt');
    
    return { blob, fileName };
  } catch (error) {
    console.error('Erro ao baixar hino em formato texto:', error);
    throw error;
  }
}

// GET /api/hymns/{id}/download/holyrics - Download hino em formato Holyrics
export async function downloadHymnHolyrics(id: string): Promise<DownloadResult> {
  try {
    const hymnId = parseHymnId(id);
    const url = API_URL ? `${API_URL}/api/hymns/${hymnId}/download/holyrics` : `/api/hymns/${hymnId}/download/holyrics`;
    const response = await fetch(url, {
      headers: getAuthHeaders(),
    });
    
    if (!response.ok) {
      if (response.status === 404) {
        throw new Error('Hino não encontrado');
      }
      throw new Error(`Erro ao baixar hino: ${response.statusText}`);
    }
    
    const blob = await response.blob();
    const fileName = extractFileNameFromHeaders(response.headers, 'hino-holyrics.txt');
    
    return { blob, fileName };
  } catch (error) {
    console.error('Erro ao baixar hino em formato Holyrics:', error);
    throw error;
  }
}

/**
 * Parseia texto de letra com marcadores [V1], [R], etc. para estrutura de versos
 * Exemplo:
 * [V1]
 * Linha 1
 * Linha 2
 * [R]
 * Refrão linha 1
 */
export function parseLyricsText(text: string): Verse[] {
  const verses: Verse[] = [];
  const lines = text.split('\n').map(line => line.trim()).filter(line => line);
  
  let currentVerse: Verse | null = null;
  
  for (const line of lines) {
    // Verifica se é um marcador de tipo de verso
    const verseTypeMatch = line.match(/^\[([V1-4]|R|Ponte|C)\]$/i);
    
    if (verseTypeMatch) {
      // Salva o verso anterior se existir
      if (currentVerse && currentVerse.lines.length > 0) {
        verses.push(currentVerse);
      }
      
      // Cria novo verso
      const type = verseTypeMatch[1].toUpperCase() as Verse['type'];
      currentVerse = {
        type: type,
        lines: []
      };
    } else if (currentVerse) {
      // Adiciona linha ao verso atual
      currentVerse.lines.push(line);
    } else {
      // Se não há verso atual e a linha não é marcador, cria verso V1 por padrão
      if (verses.length === 0) {
        currentVerse = {
          type: 'V1',
          lines: [line]
        };
      } else {
        // Adiciona à última linha do último verso se não há marcador
        if (verses.length > 0) {
          verses[verses.length - 1].lines.push(line);
        }
      }
    }
  }
  
  // Adiciona o último verso se existir
  if (currentVerse && currentVerse.lines.length > 0) {
    verses.push(currentVerse);
  }
  
  return verses;
}
