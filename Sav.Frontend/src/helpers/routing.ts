export enum PageTypes {
    Home = "/",
    CompletedGames = "/CompletedGames",
    ClosestAchievements = "/ClosestAchievements",
    AllClosestAchievements = "/AllClosestAchievements",
}

export const redirect = (path: PageTypes) => {
    window.location.href = path.toString();
}

export const redirectWithParams = (path: PageTypes, params: string) => {
    window.location.href = path.toString() + "/" + params;
}

export const redirectWithRecordParams = (path: PageTypes, params: Record<string, string>, endParam: string = "") => 
{
    let url = path.toString(); 
    for (const key in params) {
        if(url.toString().includes(":" + key))
        {
            url = url.replace(":" + key, params[key]);
        }
    }
    if(endParam != "")
    {
        url += "/" + endParam;
    }
    window.location.href = url;
}