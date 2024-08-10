using Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SteamAchievementViewer.Models.SteamApi
{

    [XmlRoot(ElementName = "gamesList")]
    public class GamesList
    {
        [XmlElement(ElementName = "steamID64")]
        public string SteamID64 { get; set; }
        [XmlElement(ElementName = "steamID")]
        public string SteamID { get; set; }
        [XmlElement(ElementName = "games")]
        public Games Games { get; set; }
    }

    [XmlRoot(ElementName = "games")]
    public class Games
    {
        [XmlElement(ElementName = "game")]
        public List<Game> Game { get; set; }
    }

    [XmlRoot(ElementName = "game")]
    public class Game
    {
        [XmlElement(ElementName = "appID")]
        [JsonProperty("appid")]
        public string AppID { get; set; }

        [XmlElement(ElementName = "name")]
        [JsonProperty("name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "gameIcon")]
        public string GameIcon { get; set; }

        [XmlElement(ElementName = "logo")]
        public string Logo { get; set; }

        [XmlElement(ElementName = "gameLogoSmall")]
        public string GameLogoSmall { get; set; }

        [XmlElement(ElementName = "storeLink")]
        public string StoreLink { get; set; }

        [XmlElement(ElementName = "hoursLast2Weeks")]
        [JsonProperty("playtime_2weeks")]
        public string HoursLast2Weeks { get; set; }

        [XmlElement(ElementName = "hoursOnRecord")]
        [JsonProperty("playtime_forever")]
        public string HoursOnRecord { get; set; }

        [XmlElement(ElementName = "statsLink")]
        public string StatsLink { get; set; }

        [XmlElement(ElementName = "globalStatsLink")]
        public string GlobalStatsLink { get; set; }

        [JsonProperty("has_community_visible_stats")]
        public bool HasCommunityVisibleStats { get; set; }

        [XmlElement(ElementName = "achievements")]
        public Achievements Achievements { get; set; } = new Achievements();
    }
}
