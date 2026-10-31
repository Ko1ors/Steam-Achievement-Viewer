export interface SteamUser {
    steamID64: string;
    steamID: string;
    customURL?: string;
    realname?: string;
    avatarIcon?: string;
    avatarMedium?: string;
    avatarFull?: string;
    avatarFrame?: string | null;
    memberSince?: string;
    onlineState?: string;
}
