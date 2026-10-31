import { queueAchievementsUpdateAsync, updateGamesAsync, updateProfileAsync } from '@/api/api';
import type { GamesUpdateResult } from '@/models/API/GamesUpdateResult';
import type { ProfileUpdateResult } from '@/models/API/ProfileUpdateResult';

export type SteamSyncStage = 'profile' | 'games' | 'sync';

/** Failed stage of the sync; a null result means the server could not be reached. */
export type SteamSyncFailure =
    | { stage: 'profile'; result: ProfileUpdateResult | null }
    | { stage: 'games'; result: GamesUpdateResult | null }
    | { stage: 'sync' };

export const PROFILE_ERRORS: Record<ProfileUpdateResult, string> = {
    Success: '',
    NotFound: 'Steam ID not found. Check the number and try again.',
    Private: 'Your Steam profile is private. Set your profile and game details to public, then try again.',
    Failed: 'Failed to get profile information. Check that the Steam ID is correct and the profile is public.',
};

export const GAMES_ERRORS: Record<GamesUpdateResult, string> = {
    Success: '',
    MissingApiKey: 'A Steam Web API key is required to read your game list. Add your Steam Web API key in Settings and try again.',
    Failed: 'Failed to get the game list. Make sure your profile and game details are public, then try again.',
};

/**
 * Loads the profile and game list for a Steam ID and queues the achievement sync.
 * Returns the failed stage, or null once the sync is running.
 */
export const syncSteamIdAsync = async (
    steamId: string,
    onStage?: (stage: SteamSyncStage) => void,
): Promise<SteamSyncFailure | null> => {
    onStage?.('profile');
    const profileResponse = await updateProfileAsync(steamId);

    if (!profileResponse.success)
        return { stage: 'profile', result: null };

    if (profileResponse.data !== 'Success')
        return { stage: 'profile', result: profileResponse.data ?? 'Failed' };

    onStage?.('games');
    const gamesResponse = await updateGamesAsync(steamId);

    if (!gamesResponse.success)
        return { stage: 'games', result: null };

    if (gamesResponse.data !== 'Success')
        return { stage: 'games', result: gamesResponse.data ?? 'Failed' };

    onStage?.('sync');
    const queueResponse = await queueAchievementsUpdateAsync();

    if (!queueResponse.success)
        return { stage: 'sync' };

    return null;
};
