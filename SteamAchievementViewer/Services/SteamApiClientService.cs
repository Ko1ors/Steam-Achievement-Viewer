using Newtonsoft.Json;
using Sav.Common.Logs;
using SteamAchievementViewer.Models.SteamApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SteamAchievementViewer.Services
{
    public class SteamApiClientService : ISteamApiClientService
    {
        public static readonly string SteamApiBaseUrl = "https://api.steampowered.com";
        public static readonly string SteamApiGetOwnedGames = "/IPlayerService/GetOwnedGames/v0001/?key={0}&steamid={1}&format=json&include_appinfo=1&include_played_free_games=1&skip_unvetted_apps=0&format=json";
        private static string ApiKeyParameterPattern = @"(?<=\bkey=)[^&]*";


        public async Task<IEnumerable<Game>> GetOwnedGamesAsync(string steamId, string steamApiKey)
        {
            var defaultResult = Enumerable.Empty<Game>();
            try
            {
                var requestUrl = string.Format(SteamApiGetOwnedGames, steamApiKey, steamId);
                var responseString = await SendGetRequest(SteamApiBaseUrl + requestUrl);
                if(string.IsNullOrWhiteSpace(responseString))
                    return defaultResult;

                var response = JsonConvert.DeserializeObject<GetOwnedGamesResponse>(responseString);

                if(response.Response == null)
                    return defaultResult;

                foreach (var game in response.Response.Games)
                {
                    game.StoreLink = $"https://store.steampowered.com/app/{game.AppID}";
                    if (game.HasCommunityVisibleStats)
                    {
                        game.GlobalStatsLink = $"https://steamcommunity.com/stats/{game.AppID}/achievements";
                        game.StatsLink = $"https://steamcommunity.com/profiles/{steamId}/stats/{game.AppID}";
                    }
                    game.Logo = $"https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/{game.AppID}/capsule_184x69.jpg ";
                }

                return response.Response.Games;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Failed to get owned games for {SteamId}", steamId);
                return Enumerable.Empty<Game>();
            }
        }

        public async Task<string> SendGetRequest(string requestUrl)
        {
            try
            {
                using var client = new HttpClient();
                var response = await client.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();
                Log.Logger.Information("Request successful, {RequestUrl}", Regex.Replace(requestUrl, ApiKeyParameterPattern, "***"));
                return await response.Content.ReadAsStringAsync();

            }
            catch (Exception e)
            {
                Log.Logger.Error(e, "Failed to request {RequestUrl}", Regex.Replace(requestUrl, ApiKeyParameterPattern, "***"));
                return string.Empty;
            }
        }


    }
}
