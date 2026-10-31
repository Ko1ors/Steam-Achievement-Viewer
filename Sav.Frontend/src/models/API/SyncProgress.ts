export interface SyncProgress {
    isRunning: boolean;
    totalGames: number;
    processedGames: number;
    queueSize: number;
    lastGameName: string | null;
    progressPercent: number;
    startedAt: string | null;
    finishedAt: string | null;
}
