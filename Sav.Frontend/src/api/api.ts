import { CompletedGameComposite } from "@/models/API/CompletedGameComposite";
import { PagedResult } from "@/models/API/PagedResult";
import moment from "moment";

export const baseUrl = import.meta.env.VITE_SERVICE_API_URL ?? window.location.origin;
export const apiUrl = baseUrl + "/api";

export interface Response<T> {
    success: boolean;
    message: string;
    status?: number;
    data: T | null;
}

// Games

export const getCompletedGamesAsync = async function(steamId: string, take: number = 25, skip: number = 0) : Promise<Response<PagedResult<CompletedGameComposite>>> {
    const response = await getAsync<PagedResult<CompletedGameComposite>>(apiUrl + "/games/GetCompletedGames?steamId=" + steamId + "&take=" + take + "&skip=" + skip);
    return response;
}



// Shared

const getResponseContentAsync = async function(response: globalThis.Response) {
    const text = await response.text();
    try {
        return JSON.parse(text);
    } catch (e) {
        return text;
    }  
}

export const getAsync = async function <T>(url: string, options?: RequestInit, ignoreRedirect: boolean = false) {
    try {
        const response = await fetch(url, {
            credentials: "include",
            redirect: 'manual',
            ...options
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
            const errorResponse: Response<T> = { success: false, message: isRedirect ? "Redirect" : await response.text(), data: null };
            console.error(errorResponse);
            return errorResponse;
        }
        return { success: true, message: "", data: await getResponseContentAsync(response) } as Response<T>;
    }
    catch (error) {
        console.error(error);
        return { success: false, message: error, data: null } as Response<T>;
    }
}

export const postAsync = async function <T>(url: string, body: any, options?: RequestInit) {
    const response = await fetch(url, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(body),
        credentials: "include",
        ...options
    });
    if (!response.ok) {
        const errorResponse: Response<T> = { success: false, message: await response.text(), data: null, status: response.status };
        console.error(errorResponse);
        return errorResponse;
    }
    return { success: true, message: "", data: await getResponseContentAsync(response) };
}

export const putAsync = async function <T>(url: string, body: any, options?: RequestInit) {
    const response = await fetch(url, {
        method: "PUT",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(body),
        credentials: "include",
        ...options
    });
    if (!response.ok) {
        const errorResponse: Response<T> = { success: false, message: await response.text(), data: null };
        console.error(errorResponse);
        return errorResponse;
    }
    return { success: true, message: "", data: await getResponseContentAsync(response) };
}

export const deleteAsync = async function <T>(url: string, body: any, options?: RequestInit) {
    const response = await fetch(url, {
        method: "DELETE",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(body),
        credentials: "include",
        ...options
    });
    if (!response.ok) {
        const errorResponse: Response<T> = { success: false, message: await response.text(), data: null };
        console.error(errorResponse);
        return errorResponse;
    }
    return { success: true, message: "", data: await getResponseContentAsync(response) };
}
