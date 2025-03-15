export interface AchievementDto {
    iconClosed: string;
    iconOpen: string;
    name: string;
    aApiname: string;
    description: string;
    percent: number;
    appID: string;
    unlockTime: string | null;
}