export interface UserSettings {
    steamId: string | null;
    steamApiKey: string | null;
    autoUpdateIntervalMinutes: number;
    updateOnlyRecentGames: boolean;
}
