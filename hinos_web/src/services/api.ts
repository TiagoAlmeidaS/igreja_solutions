import { Hymn, Verse } from '../types/hymn';

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000';

export interface ApiError {
  message: string;
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
    
    throw new Error(errorMessage);
  }
  
  // Para respostas 204 No Content, retornar void
  if (response.status === 204) {
    return undefined as T;
  }
  
  return response.json();
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

    const url = `${API_URL}/api/hymns${params.toString() ? `?${params.toString()}` : ''}`;
    const response = await fetch(url);
    const apiHymns = await handleResponse<ApiHymn[]>(response);
    return apiHymns.map(mapApiHymnToHymn);
  } catch (error) {
    console.error('Erro ao buscar hinos:', error);
    throw error;
  }
}

// GET /api/hymns/{id} - Busca hino por ID
export async function getHymnById(id: string): Promise<Hymn | null> {
  try {
    const response = await fetch(`${API_URL}/api/hymns/${id}`);
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
  if (!term.trim()) {
    return getAllHymns();
  }

  try {
    const params = new URLSearchParams({ term: term.trim() });
    const response = await fetch(`${API_URL}/api/hymns/search?${params.toString()}`);
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
    const response = await fetch(`${API_URL}/api/hymns`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
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
    const response = await fetch(`${API_URL}/api/hymns/${id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
      },
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
    const response = await fetch(`${API_URL}/api/hymns/${id}`, {
      method: 'DELETE',
    });
    
    await handleResponse<void>(response);
  } catch (error) {
    console.error('Erro ao deletar hino:', error);
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
