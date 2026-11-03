export interface UserProfile {
    steamID64: string;
    steamID: string;
    customURL?: string;
    avatarIcon?: string;
    avatarMedium?: string;
    avatarFull?: string;
    avatarFrame?: string | null;
    onlineState?: string | null;
    headline?: string | null;
    realName?: string | null;
    location?: string | null;
    memberSince?: string | null;
    gamesCount: number;
    totalAchievementsCount: number;
    completedAchievementsCount: number;
}
