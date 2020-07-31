using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Serialization;

namespace AchievementTest
{
    public static class Manager
    {
        public static Profile profile;
        public static GamesList gamesList;

        public static int currentGameRetrieve;

        private static readonly string xmlProfileError = "The specified profile could not be found.";
        private static readonly string directoryPath = AppDomain.CurrentDomain.BaseDirectory;

        private static Dictionary<Type, Page> pagesDictionary = new Dictionary<Type, Page>();

        public static void Start()
        {
            if(Settings.Default.SteamID != "-1")
            {
                LoadProfile(Settings.Default.SteamID);
                LoadGames();
            }
        }

        public static Page GetPageObject<T>() where T : Page, new()
        {
            if (!pagesDictionary.TryGetValue(typeof(T), out Page page))
            {
                page = new T();
                pagesDictionary.Add(typeof(T), page);
            }
            return page;
        }

        public static void SaveSettingsInfo()
        {
            if(profile != null)
            {
                Settings.Default.SteamID = profile.SteamID64;
                Settings.Default.LastUpdate = DateTime.Now;
                Settings.Default.Save();
            }
        }

        public static bool GetProfile(string steamID)
        {
            var response = GetRequest.XmlRequest("https://steamcommunity.com/profiles/" + steamID + "/?xml=1");
            if (response.InnerText == xmlProfileError)
                return false;
            XmlSerializer serializer = new XmlSerializer(typeof(Profile));
            using (XmlReader reader = new XmlNodeReader(response))
            {
                profile = (Profile)serializer.Deserialize(reader);
            }
            if (profile?.PrivacyState != "public")
            {
                profile = null;
                return false;
            }
            return true;
        }

        public static void SaveProfile()
        {
            if (!System.IO.Directory.Exists(directoryPath + profile.SteamID64))
                Directory.CreateDirectory(directoryPath + profile.SteamID64);
            XmlSerializer serializer = new XmlSerializer(typeof(Profile));
            System.IO.FileStream file = System.IO.File.Create(directoryPath + profile.SteamID64 + "\\profile.xml");
            serializer.Serialize(file, profile);
        }

        private static void LoadProfile(string steamID)
        {
            if (File.Exists(directoryPath + steamID + "\\profile.xml"))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Profile));
                System.IO.StreamReader file = new System.IO.StreamReader(directoryPath + steamID + "\\profile.xml");
                profile = (Profile)serializer.Deserialize(file);
                file.Close();
            }
        }

        public static bool GetGames()
        {
            var response = GetRequest.XmlRequest("https://steamcommunity.com/profiles/" + profile.SteamID64 + "/games?tab=all&xml=1");
            if (response.InnerText == xmlProfileError)
                return false;
            XmlSerializer serializer = new XmlSerializer(typeof(GamesList));
            using (XmlReader reader = new XmlNodeReader(response))
            {
                gamesList = (GamesList)serializer.Deserialize(reader);
            }
            if (gamesList?.Games.Game.Count == 0)
            {
                gamesList = null;
                return false;
            }
            CheckGamesForAchievements();
            return true;
        }

        private static void CheckGamesForAchievements()
        {
            gamesList.Games.Game.RemoveAll(e => e.GlobalStatsLink == null || e.StatsLink == null);
        }

        private static void CheckAchievementsNull()
        {
            gamesList.Games.Game.RemoveAll(e => e.Achievements == null);
        }

        public static void SaveGames()
        {
            if (!System.IO.Directory.Exists(directoryPath + profile.SteamID64))
                Directory.CreateDirectory(directoryPath + profile.SteamID64);
            XmlSerializer serializer = new XmlSerializer(typeof(GamesList));
            System.IO.FileStream file = System.IO.File.Create(directoryPath + profile.SteamID64 + "\\gameslist.xml");
            serializer.Serialize(file, gamesList);
        }

        private static void LoadGames()
        {
            if (File.Exists(directoryPath + profile.SteamID64 + "\\gameslist.xml"))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(GamesList));
                System.IO.StreamReader file = new System.IO.StreamReader(directoryPath + profile.SteamID64 + "\\gameslist.xml");
                gamesList = (GamesList)serializer.Deserialize(file);
                file.Close();
            }
        }

        public static bool GetAchievements()
        {
            currentGameRetrieve = 0;
            XmlSerializer serializer = new XmlSerializer(typeof(Achievements));
            foreach (Game game in gamesList.Games.Game)
            {
                currentGameRetrieve++;
                var response = GetRequest.XmlRequest(game.StatsLink + "/?xml=1");
                if (response.InnerText == xmlProfileError || response.InnerText == "")
                    continue;
                using (XmlReader reader = new XmlNodeReader(response))
                {
                    reader.ReadToDescendant("achievements");
                    game.Achievements = (Achievements)serializer.Deserialize(reader);
                }
                if (game.Achievements == null)
                    continue;
                Achievements achievements = GetGlobalAchievementPercentages(game.AppID);
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
            }
            CheckAchievementsNull();
            SaveGames();
            return true;
        }

        public static bool GetAchievementsParallel()
        {
            currentGameRetrieve = 0;
            Parallel.ForEach(gamesList.Games.Game, new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount * 10 }, game =>
           {
               currentGameRetrieve++;
               var response = GetRequest.XmlRequest(game.StatsLink + "/?xml=1");
               if (response.InnerText == xmlProfileError || response.InnerText == "")
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
               Achievements achievements = GetGlobalAchievementPercentages(game.AppID);
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
           });
            CheckAchievementsNull();
            return true;
        }

        public static Achievements GetGlobalAchievementPercentages(string appid)
        {
            Achievements achievements = null;
            var response = GetRequest.XmlRequest("https://api.steampowered.com/ISteamUserStats/GetGlobalAchievementPercentagesForApp/v0002/?gameid=" + appid + "&format=xml");
            using (XmlReader reader = new XmlNodeReader(response))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Achievements));
                reader.ReadToDescendant("achievements");
                achievements = (Achievements)serializer.Deserialize(reader);
            }
            return achievements;
        }

        public static Achievements GetClosestAchievements(string appid)
        {
            Achievements achievements = new Achievements
            {
                Achievement = gamesList.Games.Game.Find(e => e.AppID == appid)?.Achievements.Achievement.Where(e => e.Closed == "0").OrderByDescending(e => e.Percent).ToList()
            };
            return achievements;
        }

        public static List<Achievement> GetAllAchievementsList()
        {
            List<Achievement> achList = new List<Achievement>();
            gamesList.Games.Game.ForEach(e =>achList.AddRange(e.Achievements.Achievement));
            return achList;
        }

        public static List<AchievementWithGameInfo> GetAllAchievementsWithGameInfoList()
        {
            List<AchievementWithGameInfo> achList = new List<AchievementWithGameInfo>();
            gamesList.Games.Game.ForEach(e =>
            {
                achList.AddRange(e.Achievements.Achievement.ConvertAll(e => new AchievementWithGameInfo(e))
                    .Select(i => { i.GameIcon = e.GameIcon; i.GameName = e.Name; return i; }));
            });
            return achList;
        }

        public static List<Achievement> GetRarestAchievements()
        {
            return GetAllAchievementsList().OrderBy(e => e.Percent).Where(e => e.Closed == "0").ToList();
        }

        public static List<Achievement> GetRarestAchievements(int count)
        {
            return GetAllAchievementsList().OrderBy(e => e.Percent).Where(e => e.Closed == "0").Take(count).ToList();
        }

        public static List<AchievementWithGameInfo> GetRarestAchievementsWithGameInfo(int count)
        {
            return GetAllAchievementsWithGameInfoList().OrderBy(e => e.Percent).Where(e => e.Closed == "0").Take(count).ToList();
        }

        public static List<AchievementWithGameInfo> GetLatestAchievements(int count)
        {
            return GetAllAchievementsWithGameInfoList().OrderByDescending(e => e.UnlockTime).Where(e => e.Closed == "1").Take(count).ToList();
        }
    }
}
