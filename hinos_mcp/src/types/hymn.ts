// Hymn-related types for MCP tools

export type HymnCategory = 'hinario' | 'canticos' | 'suplementar' | 'novos';

export interface HymnListParams {
  category?: HymnCategory | string;
  search?: string;
}

export interface HymnGetParams {
  id: number;
}

export interface HymnSearchParams {
  term: string;
}

export interface HymnCreateParams {
  number: string;
  title: string;
  category: string;
  hymnBook: string;
  key?: string;
  bpm?: number;
  verses?: Array<{
    type: string;
    lines: string[];
  }>;
}

export interface HymnUpdateParams {
  id: number;
  number: string;
  title: string;
  category: string;
  hymnBook: string;
  key?: string;
  bpm?: number;
  verses?: Array<{
    type: string;
    lines: string[];
  }>;
}

export interface HymnDeleteParams {
  id: number;
}

