export interface WarCry {
  id: number;
  title: string;
  fileName: string;
  content: string;
  messageNumber: number;
  theme?: string;
  sourcePath: string;
  fileSize: number;
  fileModifiedAt?: string;
  syncedAt: string;
  createdAt: string;
  updatedAt: string;
}

export interface WarCryListItem {
  id: number;
  title: string;
  messageNumber: number;
  theme?: string;
  contentPreview: string;
  syncedAt: string;
}

export interface WarCrySyncStatus {
  isRunning: boolean;
  lastSyncStart?: string;
  lastSyncEnd?: string;
  totalWarCries: number;
  newWarCries: number;
  updatedWarCries: number;
  failedCount: number;
  lastError?: string;
  recentFiles: string[];
}
