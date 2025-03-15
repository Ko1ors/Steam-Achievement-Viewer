export interface UserGameDto {
    userId: string;
    appID: string;
    statsLink: string | null;
    hoursLast2Weeks: string | null;
    hoursOnRecord: string | null;
}