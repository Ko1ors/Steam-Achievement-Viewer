using AutoMapper;
using Sav.Common.Interfaces;
using Sav.Infrastructure.Entities;
using SteamAchievementViewer.Mapping;
using SteamAchievementViewer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace SteamAchievementViewer.Services
{
    public class AchievementsWorkerService : IAchievementsWorkerService
    {
        private const string XmlProfileError = "The specified profile could not be found.";
        private static readonly TimeSpan GameIconsUpdateInterval = TimeSpan.FromHours(24);

        private readonly IQueueService<UserGameEntity> _queueService;
        private readonly IEntityRepository<GameEntity> _gameRepository;
        private readonly IEntityRepository<AchievementEntity> _achievementRepository;
        private readonly IMapper _mapper;
        private readonly IClientService<XmlDocument> _xmlClient;
        private readonly Random _random;

        public bool IsStarted { get; set; }

        public bool IsRunning { get; set; }

        public AchievementsWorkerService(IQueueService<UserGameEntity> queueService, IEntityRepository<GameEntity> gameRepository,
            IEntityRepository<AchievementEntity> achievementRepository, IMapper mapper, IClientService<XmlDocument> xmlClient)
        {
            _queueService = queueService;
            _gameRepository = gameRepository;
            _achievementRepository = achievementRepository;
            _mapper = mapper;
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
            UserGameEntity userGame = null;
            try
            {
                if (_queueService.Size == 0)
                    return;

                userGame = _queueService.Get();
                if (userGame is null)
                    return;

                var response = await _xmlClient.SendGetRequest($"{userGame.StatsLink}/?xml={GetRandomParameter()}");
                if (response.InnerText == XmlProfileError || response.InnerText == "")
                    return;
                Achievements achievementsResponse = null;
                List<AchievementEntity> achievements = null;
                using (XmlReader reader = new XmlNodeReader(response))
                {
                    var serializer = new XmlSerializer(typeof(Game));
                    reader.ReadToDescendant("game");
                    var game_ = (Game)serializer.Deserialize(reader);
                    serializer = new XmlSerializer(typeof(Achievements));
                    reader.ReadToNextSibling("achievements");
                    achievementsResponse = (Achievements)serializer.Deserialize(reader);

                    var gameEntity = _gameRepository.GetByKeys(userGame.AppID);
                    achievements = _mapper.MapMultiple<List<AchievementEntity>>(achievementsResponse.Achievement, gameEntity);

                    if (DateTime.Now - gameEntity.Updated > GameIconsUpdateInterval)
                    {
                        gameEntity.GameLogoSmall = game_.GameLogoSmall;
                        gameEntity.GameIcon = game_.GameIcon;
                        await _gameRepository.UpdateAsync(gameEntity);
                    }
                }
                if (achievements == null)
                    return;

                // TODO: move GetGlobalAchievementPercentages to separate worker service
                achievementsResponse = await GetGlobalAchievementPercentagesAsync(userGame.AppID);
                if (achievementsResponse != null)
                {
                    achievements = achievements.GroupJoin(achievementsResponse.Achievement,
                        e => e.Apiname.ToLower(),
                        a => a.Name.ToLower(),
                        (e, a) =>
                        {
                            e.Percent = a.FirstOrDefault()?.Percent ?? -1;
                            return e;
                        }).ToList();
                }

                foreach (var achievement in achievements)
                {
                    _achievementRepository.AddOrUpdate(achievement);
                }
            }
            catch (Exception e)
            {
                if (userGame is not null)
                    _queueService.Add(userGame);
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
