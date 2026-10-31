export interface UserProfile {
    steamID64: string;
    steamID: string;
    customURL?: string;
    avatarIcon?: string;
    avatarMedium?: string;
    avatarFull?: string;
    avatarFrame?: string | null;
    gamesCount: number;
    totalAchievementsCount: number;
    completedAchievementsCount: number;
}
