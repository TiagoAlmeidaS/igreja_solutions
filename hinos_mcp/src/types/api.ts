// API Request/Response types matching hinos_api DTOs

export interface LoginRequest {
  email: string;
  password: string;
}

export interface User {
  id: string;
  name: string;
  email: string;
}

export interface LoginResponse {
  token: string;
  user: User;
}

export interface VerseInput {
  type: string;
  lines: string[];
}

export interface Verse {
  type: string;
  lines: string[];
}

export interface CreateHymnDto {
  number: string;
  title: string;
  category: string;
  hymnBook: string;
  key?: string;
  bpm?: number;
  verses: VerseInput[];
}

export interface UpdateHymnDto {
  number: string;
  title: string;
  category: string;
  hymnBook: string;
  key?: string;
  bpm?: number;
  verses: VerseInput[];
}

export interface HymnResponse {
  id: number;
  number: string;
  title: string;
  category: string;
  hymnBook: string;
  key?: string;
  bpm?: number;
  verses: Verse[];
}

