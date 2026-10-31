import { AchievementComposite } from "@/models/API/AchievementComposite";
import { CompletedGameComposite } from "@/models/API/CompletedGameComposite";
import { CompletionGameComposite } from "@/models/API/CompletionGameComposite";
import { CurrentUser } from "@/models/API/CurrentUser";
import { GameDto } from "@/models/API/GameDto";
import { PagedResult } from "@/models/API/PagedResult";
import { ProfileUpdateResult } from "@/models/API/ProfileUpdateResult";
import { GamesUpdateResult } from "@/models/API/GamesUpdateResult";
import { UserProfile } from "@/models/API/UserProfile";
import { notifyError } from "@/helpers/notifications";

export const baseUrl = import.meta.env.VITE_SERVICE_API_URL ?? window.location.origin;
export const apiUrl = baseUrl + "/api";

export interface Response<T> {
    success: boolean;
    message: string;
    status?: number;
    data: T | null;
}

/** Requests stay silent when the caller renders its own fallback state (e.g. the shell header). */
export interface RequestOptions extends RequestInit {
    silent?: boolean;
}


// Games

export const getCompletedGamesAsync = async function(steamId?: string, take: number = 25, skip: number = 0) : Promise<Response<PagedResult<CompletedGameComposite>>> {
    const response = await getAsync<PagedResult<CompletedGameComposite>>(apiUrl + "/games/GetCompletedGames" + toQueryString({ steamId, take, skip }));
    return response;
}

export const getIncompleteGamesAsync = async function(steamId?: string, take: number = 25, skip: number = 0) : Promise<Response<GameDto[]>> {
    const response = await getAsync<GameDto[]>(apiUrl + "/games/GetIncompleteGames" + toQueryString({ steamId, take, skip }));
    return response;
}

export const getGameClosestAchievementsAsync = async function(steamId: string | undefined, gameId: string) : Promise<Response<AchievementComposite[]>> {
    const response = await getAsync<AchievementComposite[]>(apiUrl + "/games/GetGameClosestAchievements" + toQueryString({ steamId, appid: gameId }));
    return response;
}

export const getClosestAchievementsAsync = async function(steamId?: string, take: number = 25, skip: number = 0) : Promise<Response<AchievementComposite[]>> {
    const response = await getAsync<AchievementComposite[]>(apiUrl + "/games/GetClosestAchievements" + toQueryString({ steamId, take, skip }));
    return response;
}

export const getEasiestGamesToCompleteAsync = async function(steamId?: string, page: number = 1, count: number = 25) : Promise<Response<PagedResult<CompletionGameComposite>>> {
    const response = await getAsync<PagedResult<CompletionGameComposite>>(apiUrl + "/games/GetPagedEasiestGamesToComplete" + toQueryString({ steamId, page, count }));
    return response;
}

export const getRarestAchievementsAsync = async function(steamId?: string, page: number = 1, count: number = 100) : Promise<Response<AchievementComposite[]>> {
    const response = await getAsync<AchievementComposite[]>(apiUrl + "/achievements/GetRarestAchievements" + toQueryString({ steamId, page, count }));
    return response;
}

export const getLatestAchievementsAsync = async function(steamId?: string, page: number = 1, count: number = 100) : Promise<Response<AchievementComposite[]>> {
    const response = await getAsync<AchievementComposite[]>(apiUrl + "/achievements/GetLatestAchievements" + toQueryString({ steamId, page, count }));
    return response;
}

// Steam

export const getCurrentUserAsync = async function() : Promise<Response<CurrentUser>> {
    return await getAsync<CurrentUser>(apiUrl + "/steam/currentUser", { silent: true });
}

export const updateCurrentUserAsync = async function(steamId: string) : Promise<Response<CurrentUser>> {
    return await postAsync<CurrentUser>(apiUrl + "/steam/currentUser", { steamId });
}

export const getUserProfileAsync = async function(steamId?: string) : Promise<Response<UserProfile>> {
    return await getAsync<UserProfile>(apiUrl + "/steam/profile" + toQueryString({ steamId }));
}

export const getQueueSizeAsync = async function() : Promise<Response<number>> {
    return await getAsync<number>(apiUrl + "/steam/queueSize", { silent: true });
}

export const updateProfileAsync = async function(steamId: string) : Promise<Response<ProfileUpdateResult>> {
    return await postAsync<ProfileUpdateResult>(apiUrl + "/steam/updateProfile", steamId, { silent: true });
}

export const updateGamesAsync = async function(steamId: string) : Promise<Response<GamesUpdateResult>> {
    return await postAsync<GamesUpdateResult>(apiUrl + "/steam/updateGames", steamId, { silent: true });
}

export const queueAchievementsUpdateAsync = async function(onlyRecentGames: boolean = true) : Promise<Response<unknown>> {
    return await postAsync<unknown>(apiUrl + "/steam/queueAchievementsUpdate" + toQueryString({ onlyRecentGames }), {}, { silent: true });
}



// Shared

const toQueryString = function(params: Record<string, string | number | boolean | undefined>) {
    const query = new URLSearchParams();
    Object.entries(params).forEach(([key, value]) => {
        if (value !== undefined && value !== '')
            query.append(key, String(value));
    });
    const result = query.toString();
    return result ? `?${result}` : '';
}

const getResponseContentAsync = async function(response: globalThis.Response) {
    const text = await response.text();
    try {
        return JSON.parse(text);
    } catch (e) {
        return text;
    }  
}

export const getAsync = async function <T>(url: string, options?: RequestOptions, ignoreRedirect: boolean = false) {
    const { silent, ...init } = options ?? {};
    try {
        const response = await fetch(url, {
            credentials: "include",
            redirect: 'manual',
            ...init
        })
        const isRedirect = response.type === "opaqueredirect";
        if (!ignoreRedirect && isRedirect) {
            const redirectUrl = response.url;
            if (redirectUrl) {
                window.open(redirectUrl, '_blank'); // Open in a new tab/window
            } else {
                console.error('Redirect URL not found in headers.');
            }
            return { success: false, message: "Redirect", data: null } as Response<T>;
        }
        if (!response.ok) {
            const message = isRedirect ? "Redirect" : await response.text();
            const errorResponse: Response<T> = { success: false, message, data: null, status: response.status };
            if (!silent) {
                console.error(errorResponse);
                notifyError("Could not load data", message.slice(0, 300) || undefined);
            }
            return errorResponse;
        }
        return { success: true, message: "", data: await getResponseContentAsync(response) } as Response<T>;
    }
    catch (error) {
        if (!silent) {
            console.error(error);
            notifyError("Could not reach the server", error instanceof Error ? error.message : String(error));
        }
        return { success: false, message: error, data: null } as Response<T>;
    }
}

export const postAsync = async function <T>(url: string, body: any, options?: RequestOptions) {
    const { silent, ...init } = options ?? {};
    const response = await fetch(url, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(body),
        credentials: "include",
        ...init
    });
    if (!response.ok) {
        const message = await response.text();
        const errorResponse: Response<T> = { success: false, message, data: null, status: response.status };
        if (!silent) {
            console.error(errorResponse);
            notifyError("Could not save changes", message.slice(0, 300) || undefined);
        }
        return errorResponse;
    }
    return { success: true, message: "", data: await getResponseContentAsync(response) };
}

export const putAsync = async function <T>(url: string, body: any, options?: RequestOptions) {
    const { silent, ...init } = options ?? {};
    const response = await fetch(url, {
        method: "PUT",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(body),
        credentials: "include",
        ...init
    });
    if (!response.ok) {
        const message = await response.text();
        const errorResponse: Response<T> = { success: false, message, data: null, status: response.status };
        if (!silent) {
            console.error(errorResponse);
            notifyError("Could not save changes", message.slice(0, 300) || undefined);
        }
        return errorResponse;
    }
    return { success: true, message: "", data: await getResponseContentAsync(response) };
}

export const deleteAsync = async function <T>(url: string, body: any, options?: RequestOptions) {
    const { silent, ...init } = options ?? {};
    const response = await fetch(url, {
        method: "DELETE",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(body),
        credentials: "include",
        ...init
    });
    if (!response.ok) {
        const message = await response.text();
        const errorResponse: Response<T> = { success: false, message, data: null, status: response.status };
        if (!silent) {
            console.error(errorResponse);
            notifyError("Could not delete", message.slice(0, 300) || undefined);
        }
        return errorResponse;
    }
    return { success: true, message: "", data: await getResponseContentAsync(response) };
}
