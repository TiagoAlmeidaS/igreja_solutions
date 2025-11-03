export interface Hymn {
  id: string;
  number: string;
  title: string;
  category: 'hinario' | 'canticos' | 'suplementar' | 'novos';
  hymnBook: string;
  verses: Verse[];
  key?: string;
  bpm?: number;
}

export interface Verse {
  type: 'V1' | 'V2' | 'V3' | 'V4' | 'R' | 'Ponte' | 'C';
  lines: string[];
}

export type CategoryFilter = 'todos' | 'hinario' | 'canticos' | 'suplementar' | 'novos';
