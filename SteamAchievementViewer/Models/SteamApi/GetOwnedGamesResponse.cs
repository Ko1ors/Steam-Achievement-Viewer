using Newtonsoft.Json;
using System.Collections.Generic;

namespace SteamAchievementViewer.Models.SteamApi
{
    public class GetOwnedGamesResponse
    {
        [JsonProperty("response")]
        public GetOwnedGamesResponseResponse Response { get; set; }
    }

    public class GetOwnedGamesResponseResponse
    {
        [JsonProperty("game_count")]
        public int GameCount { get; set; }

        [JsonProperty("games")]
        public List<Game> Games { get; set; }
    }
}
