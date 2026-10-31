import type { UserProfile } from './UserProfile';

export interface CurrentUser {
    isLoggedIn: boolean;
    steamId: string | null;
    profile: UserProfile | null;
    queueSize: number;
}
