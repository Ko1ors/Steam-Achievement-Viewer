using SteamAchievementViewer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace SteamAchievementViewer.Services
{
    public class SteamService : ISteamService
    {
        private const string XmlProfileError = "The specified profile could not be found.";
        private const int UpdateNotifyStep = 5;
        private static readonly string DirectoryPath = AppDomain.CurrentDomain.BaseDirectory + "data\\";

        private readonly IClientService<XmlDocument> _xmlClient;

        public event AchievementProgressUpdatedDelegate OnAchievementProgressUpdated;

        public Profile Profile { get; private set; }

        public GamesList GamesList { get; private set; }


        public SteamService(IClientService<XmlDocument> xmlClient)
        {
            _xmlClient = xmlClient;
        }

        public async Task<bool> GetAchievementsAsync(List<Game> games)
        {
            int gameRetrieved = 0;
            int prevUpdate = 0 - UpdateNotifyStep;
            XmlSerializer serializer = new XmlSerializer(typeof(Achievements));
            foreach (Game game in GamesList.Games.Game)
            {
                var response = await _xmlClient.SendGetRequest(game.StatsLink + "/?xml=1");
                if (response.InnerText == XmlProfileError || response.InnerText == "")
                    continue;
                using (XmlReader reader = new XmlNodeReader(response))
                {
                    reader.ReadToDescendant("achievements");
                    game.Achievements = (Achievements)serializer.Deserialize(reader);
                }
                if (game.Achievements == null)
                    continue;
                Achievements achievements = await GetGlobalAchievementPercentagesAsync(game.AppID);
                if (achievements != null)
                {
                    foreach (Achievement achievement in game.Achievements.Achievement)
                    {
                        var achv = achievements.Achievement.Find(e => e.Name.ToLower() == achievement.Apiname.ToLower());
                        if (achv != null)
                            achievement.Percent = achv.Percent;
                        else
                            throw new Exception();
                    }
                }
                gameRetrieved++;
                if ((gameRetrieved - prevUpdate >= UpdateNotifyStep) || (gameRetrieved == GamesList.Games.Game.Count))
                {
                    prevUpdate = gameRetrieved;
                    OnAchievementProgressUpdated?.Invoke(GamesList.Games.Game.Count, gameRetrieved, game.Name);
                }
            }
            OnAchievementProgressUpdated?.Invoke(GamesList.Games.Game.Count, GamesList.Games.Game.Count, GamesList.Games.Game.Last().Name);
            GamesList.Games.Game.RemoveAll(e => e.Achievements == null);
            SaveGames();
            return true;
        }
        
        public async Task<bool> GetAchievementsParallelAsync(List<Game> games)
        {
            int gameRetrieved = 0;
            int prevUpdate = 0 - UpdateNotifyStep;
            await Parallel.ForEachAsync(GamesList.Games.Game, new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount * 10 }, async (game, token) =>
            {
                var response = await _xmlClient.SendGetRequest(game.StatsLink + "/?xml=1");
                if (response.InnerText == XmlProfileError || response.InnerText == "")
                    return;
                using (XmlReader reader = new XmlNodeReader(response))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Game));
                    reader.ReadToDescendant("game");
                    var game_ = (Game)serializer.Deserialize(reader);
                    serializer = new XmlSerializer(typeof(Achievements));
                    reader.ReadToNextSibling("achievements");
                    game.Achievements = (Achievements)serializer.Deserialize(reader);
                    game.GameLogoSmall = game_.GameLogoSmall;
                    game.GameIcon = game_.GameIcon;
                }
                if (game.Achievements == null)
                    return;
                Achievements achievements = await GetGlobalAchievementPercentagesAsync(game.AppID);
                if (achievements != null)
                {
                    foreach (Achievement achievement in game.Achievements.Achievement)
                    {
                        var achv = achievements.Achievement.Find(e => e.Name.ToLower() == achievement.Apiname.ToLower());
                        if (achv != null)
                            achievement.Percent = achv.Percent;
                        else
                            achievement.Percent = -1;
                    }
                }
                gameRetrieved++;
                if (gameRetrieved - prevUpdate >= UpdateNotifyStep)
                {
                    prevUpdate = gameRetrieved;
                    OnAchievementProgressUpdated?.Invoke(GamesList.Games.Game.Count, gameRetrieved, game.Name);
                }
            });
            OnAchievementProgressUpdated?.Invoke(GamesList.Games.Game.Count, GamesList.Games.Game.Count, GamesList.Games.Game.Last().Name);
            GamesList.Games.Game.RemoveAll(e => e.Achievements == null);
            SaveGames();
            return true;
        }

        public async Task<bool> GetGamesAsync(string steamID)
        {
            var response = await _xmlClient.SendGetRequest("https://steamcommunity.com/profiles/" + Profile.SteamID64 + "/games?tab=all&xml=1");
            if (response.InnerText == XmlProfileError)
                return false;
            XmlSerializer serializer = new XmlSerializer(typeof(GamesList));
            using (XmlReader reader = new XmlNodeReader(response))
            {
                GamesList = (GamesList)serializer.Deserialize(reader);
            }
            if (GamesList?.Games.Game.Count == 0)
            {
                GamesList = null;
                return false;
            }
            GamesList.Games.Game.RemoveAll(e => e.GlobalStatsLink == null || e.StatsLink == null);
            return true;
        }

        public async Task<Achievements> GetGlobalAchievementPercentagesAsync(string appid)
        {
            Achievements achievements = null;
            var response = await _xmlClient.SendGetRequest("https://api.steampowered.com/ISteamUserStats/GetGlobalAchievementPercentagesForApp/v0002/?gameid=" + appid + "&format=xml");
            using (XmlReader reader = new XmlNodeReader(response))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Achievements));
                reader.ReadToDescendant("achievements");
                achievements = (Achievements)serializer.Deserialize(reader);
            }
            return achievements;
        }

        public async Task<bool> GetProfileAsync(string steamID)
        {
            var response = await _xmlClient.SendGetRequest("https://steamcommunity.com/profiles/" + steamID + "/?xml=1");
            if (response.InnerText == XmlProfileError)
                return false;
            XmlSerializer serializer = new XmlSerializer(typeof(Profile));
            using (XmlReader reader = new XmlNodeReader(response))
            {
                Profile = (Profile)serializer.Deserialize(reader);
            }
            if (Profile?.PrivacyState != "public")
            {
                Profile = null;
                return false;
            }
            return true;
        }

        public bool IsLogged()
        {
            return Profile != null;
        }

        private void SaveGames()
        {
            if (!Directory.Exists(DirectoryPath + Profile.SteamID64))
                Directory.CreateDirectory(DirectoryPath + Profile.SteamID64);
            XmlSerializer serializer = new XmlSerializer(typeof(GamesList));
            FileStream file = File.Create(DirectoryPath + Profile.SteamID64 + "\\gameslist.xml");
            serializer.Serialize(file, GamesList);
        }
    }
}
