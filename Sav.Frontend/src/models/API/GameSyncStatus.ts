export type GameSyncState = 'Synced' | 'Syncing' | 'Failed' | 'NotSynced';

export interface GameSyncStatus {
    appId: string;
    name: string;
    status: GameSyncState;
    lastSyncedAt: string | null;
    error: string | null;
}

export interface GameSyncStatusPage {
    page: number;
    count: number;
    totalCount: number;
    syncedCount: number;
    failedCount: number;
    syncingCount: number;
    notSyncedCount: number;
    isRunning: boolean;
    queueSize: number;
    items: GameSyncStatus[];
}
