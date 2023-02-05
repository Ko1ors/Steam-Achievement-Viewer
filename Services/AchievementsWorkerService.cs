using Sav.Common.Interfaces;
using SteamAchievementViewer.Models;
using System;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace SteamAchievementViewer.Services
{
    public class AchievementsWorkerService : IAchievementsWorkerService
    {
        private const string XmlProfileError = "The specified profile could not be found.";
        
        private readonly IQueueService<Game> _queueService;
        private readonly IClientService<XmlDocument> _xmlClient;
        private readonly Random _random;

        public bool IsStarted { get; set; }
        
        public bool IsRunning { get; set; }

        public AchievementsWorkerService(IQueueService<Game> queueService, IClientService<XmlDocument> xmlClient)
        {
            _queueService = queueService;
            _xmlClient = xmlClient;
            _random = new Random();
        }

        public async Task StartAsync(CancellationToken token)
        {
            IsStarted = true;
            IsRunning = true;

            while (!token.IsCancellationRequested)
            {
                await ExecuteWorkAsync();
                await Task.Delay(1000, token);
            }
            IsRunning = false;
        }

        private int GetRandomParameter() => _random.Next(0, 100000);

        private async Task ExecuteWorkAsync()
        {
            Game game = null;
            try
            {
                if (_queueService.Size == 0)
                    return;

                game = _queueService.Get();
                if (game is null)
                    return;

                var response = await _xmlClient.SendGetRequest($"{game.StatsLink}/?xml={GetRandomParameter()}");
                if (response.InnerText == XmlProfileError || response.InnerText == "")
                    return;
                using (XmlReader reader = new XmlNodeReader(response))
                {
                    var serializer = new XmlSerializer(typeof(Game));
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
            }
            catch (Exception e)
            {
                if (game is not null)
                    _queueService.Add(game);
            }
        }

        private async Task<Achievements> GetGlobalAchievementPercentagesAsync(string appid)
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
    }
}
