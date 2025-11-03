import { Hymn } from '../types/hymn';

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000';

export interface ApiError {
  message: string;
}

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
    const error: ApiError = await response.json().catch(() => ({
      message: `Erro HTTP: ${response.status} ${response.statusText}`,
    }));
    throw new Error(error.message || `Erro HTTP: ${response.status}`);
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

export async function getAllHymns(
  category?: string,
  search?: string
): Promise<Hymn[]> {
  const params = new URLSearchParams();
  if (category && category !== 'todos') params.append('category', category);
  if (search) params.append('search', search);

  const url = `${API_URL}/api/hymns${params.toString() ? `?${params.toString()}` : ''}`;
  const response = await fetch(url);
  const apiHymns = await handleResponse<ApiHymn[]>(response);
  return apiHymns.map(mapApiHymnToHymn);
}

export async function getHymnById(id: string): Promise<Hymn | null> {
  try {
    const response = await fetch(`${API_URL}/api/hymns/${id}`);
    const apiHymn = await handleResponse<ApiHymn>(response);
    return mapApiHymnToHymn(apiHymn);
  } catch (error) {
    console.error('Erro ao buscar hino:', error);
    return null;
  }
}

export async function searchHymns(term: string): Promise<Hymn[]> {
  if (!term.trim()) {
    return getAllHymns();
  }

  const params = new URLSearchParams({ term: term.trim() });
  const response = await fetch(`${API_URL}/api/hymns/search?${params.toString()}`);
  const apiHymns = await handleResponse<ApiHymn[]>(response);
  return apiHymns.map(mapApiHymnToHymn);
}
