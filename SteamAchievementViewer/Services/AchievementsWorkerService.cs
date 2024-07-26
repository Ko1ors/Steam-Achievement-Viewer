using AutoMapper;
using Sav.Common.Interfaces;
using Sav.Common.Logs;
using Sav.Infrastructure.Entities;
using SteamAchievementViewer.Mapping;
using SteamAchievementViewer.Models.SteamApi;
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

        private readonly IQueueService<UserGameEntity> _queueService;
        private readonly ISteamService _steamService;
        private readonly IEntityRepository<GameEntity> _gameRepository;
        private readonly IEntityRepository<AchievementEntity> _achievementRepository;
        private readonly IEntityRepository<UserAchievementEntity> _userAchievementRepository;
        private readonly IMapper _mapper;
        private readonly IClientService<XmlDocument> _xmlClient;
        private readonly Random _random;

        public bool IsStarted { get; set; }

        public bool IsRunning { get; set; }

        public AchievementsWorkerService(IQueueService<UserGameEntity> queueService, ISteamService steamService, IGameEntityRepository gameRepository,
            IEntityRepository<AchievementEntity> achievementRepository, IEntityRepository<UserAchievementEntity> userAchievementRepository,
            IMapper mapper, IClientService<XmlDocument> xmlClient)
        {
            _queueService = queueService;
            _steamService = steamService;
            _gameRepository = gameRepository;
            _achievementRepository = achievementRepository;
            _userAchievementRepository = userAchievementRepository;
            _mapper = mapper;
            _xmlClient = xmlClient;
            _random = new Random();
        }

        public async Task StartAsync(CancellationToken token)
        {
            Log.Logger.Information("AchievementsWorkerService starting");
            if (IsRunning)
            {
                Log.Logger.Information("AchievementsWorkerService already running");
                return;
            }

            IsStarted = true;
            IsRunning = true;

            while (!token.IsCancellationRequested)
            {
                await ExecuteWorkAsync();
                await Task.Delay(1000, token);
            }
            Log.Logger.Warning("AchievementsWorkerService stopping");
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

                Log.Logger.Information("Processing user game {UserId}, {AppID}", userGame.UserId, userGame.AppID);

                var response = await _xmlClient.SendGetRequest($"{userGame.StatsLink}/?xml={GetRandomParameter()}");
                if (response.InnerText == XmlProfileError || response.InnerText == "")
                {
                    Log.Logger.Warning("Failed to get achievements for {UserId}, {AppID}", userGame.UserId, userGame.AppID);
                    return;
                }

                Achievements achievementsResponse = null;
                List<AchievementEntity> achievements = null;
                IEnumerable<UserAchievementEntity> userAchievements = null;
                using (XmlReader reader = new XmlNodeReader(response))
                {
                    var serializer = new XmlSerializer(typeof(Game));
                    reader.ReadToDescendant("game");
                    var game_ = (Game)serializer.Deserialize(reader);
                    serializer = new XmlSerializer(typeof(Achievements));
                    reader.ReadToNextSibling("achievements");
                    achievementsResponse = (Achievements)serializer.Deserialize(reader);
                    if (achievementsResponse is null)
                    {
                        Log.Logger.Warning("Failed to deserialize achievements for {UserId}, {AppID}", userGame.UserId, userGame.AppID);
                        return;
                    }

                    var gameEntity = _gameRepository.GetByKeys(userGame.AppID);
                    userAchievements = achievementsResponse.Achievement.Where(a => a.Closed == "1").Select(a => new UserAchievementEntity()
                    {
                        UserId = userGame.UserId,
                        AppID = userGame.AppID,
                        Apiname = a.Apiname,
                        UnlockTime = a.UnlockTime
                    });
                    achievements = _mapper.MapMultiple<List<AchievementEntity>>(achievementsResponse.Achievement, gameEntity);

                    gameEntity.GameLogoSmall = game_.GameLogoSmall;
                    gameEntity.GameIcon = game_.GameIcon;
                    await _gameRepository.UpdateAsync(gameEntity);
                    Log.Logger.Information("Game {AppID} updated, achievements count: {Count}", userGame.AppID, achievements.Count);
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
                    await _achievementRepository.AddOrUpdateAsync(achievement);
                }
                foreach (var userAchievement in userAchievements)
                {
                    await _userAchievementRepository.AddOrUpdateAsync(userAchievement);
                }
                _steamService.AchievementsDataChanged();
                Log.Logger.Information("User game {UserId}, {AppID} processed", userGame.UserId, userGame.AppID);
            }
            catch (Exception e)
            {
                Log.Logger.Error(e, "Failed to process user game {UserId}, {AppID}", userGame?.UserId, userGame?.AppID);
                if (userGame is not null)
                    _queueService.Add(userGame);
            }
        }

        private async Task<Achievements> GetGlobalAchievementPercentagesAsync(string appid)
        {
            Log.Logger.Information("Getting global achievement percentages for {AppID}", appid);
            Achievements achievements = null;
            var response = await _xmlClient.SendGetRequest("https://api.steampowered.com/ISteamUserStats/GetGlobalAchievementPercentagesForApp/v0002/?gameid=" + appid + "&format=xml");
            using (XmlReader reader = new XmlNodeReader(response))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Achievements));
                reader.ReadToDescendant("achievements");
                achievements = (Achievements)serializer.Deserialize(reader);
            }
            Log.Logger.Information("Got global achievement percentages for {AppID}", appid);
            return achievements;
        }
    }
}
