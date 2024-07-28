using AutoMapper;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Sav.Common.Interfaces;
using Sav.Common.Logs;
using Sav.Infrastructure.Entities;
using Serilog.Core;
using SteamAchievementViewer.Mapping;
using SteamAchievementViewer.Models;
using SteamAchievementViewer.Models.SteamApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Profile = SteamAchievementViewer.Models.SteamApi.Profile;

namespace SteamAchievementViewer.Services
{
    public class SteamService : ISteamService
    {
        private const string XmlProfileError = "The specified profile could not be found.";
        private static readonly TimeSpan AchievementsUpdateInterval = TimeSpan.FromHours(24);
        private static readonly TimeSpan AchievementsRecentUpdateInterval = TimeSpan.FromMinutes(5);

        private readonly Random _random;
        private readonly IClientService<XmlDocument> _xmlClient;
        private readonly IQueueService<UserGameEntity> _gameQueueService;
        private readonly IMapper _mapper;

        private readonly IUserEntityRepository _userRepository;
        private readonly IEntityRepository<GameEntity> _gameRepository;
        private readonly IEntityRepository<UserGameEntity> _userGameRepository;

        public event AchievementProgressUpdatedDelegate OnAchievementProgressUpdated;
        public event AvatarUpdatedDelegate OnAvatarUpdated;

        private string _steamID;
        private bool _refreshRequired;
        private HttpClient _httpClient;

        private HttpClient httpClient
        {
            get
            {
                _httpClient ??= new HttpClient();
                return _httpClient;
            }
        }

        public SteamService(IClientService<XmlDocument> xmlClient, IQueueService<UserGameEntity> gameQueueService, IMapper mapper,
            IUserEntityRepository userRepository, IGameEntityRepository gameRepository,
            IEntityRepository<UserGameEntity> userGameRepository)
        {
            _xmlClient = xmlClient;
            _gameQueueService = gameQueueService;
            _mapper = mapper;
            _userRepository = userRepository;
            _gameRepository = gameRepository;
            _userGameRepository = userGameRepository;

            _random = new Random();
            _refreshRequired = false;
            Start();
        }

        private bool Start()
        {
            Log.Logger.Information("SteamService started");

            if (Settings.Default.SteamID == "-1")
            {
                Log.Logger.Information("SteamID is not set");
                return false;
            }

            LoadProfile(Settings.Default.SteamID);

            Log.Logger.Information("SteamService started successfully");
            return true;
        }

        private int GetRandomParameter() => _random.Next(0, 100000);

        public async Task<bool> UpdateGamesAsync(string steamID)
        {
            List<GameEntity> games = null;
            List<UserGameEntity> userGames = null;
            var response = await _xmlClient.SendGetRequest($"https://steamcommunity.com/profiles/{steamID}/games?tab=all&xml={GetRandomParameter()}");
            if (response.InnerText == XmlProfileError)
                return false;
            var serializer = new XmlSerializer(typeof(GamesList));
            using (XmlReader reader = new XmlNodeReader(response))
            {
                var gamesList = (GamesList)serializer.Deserialize(reader);
                games = _mapper.Map<List<GameEntity>>(gamesList.Games.Game);
                var currentUser = _userRepository.GetByKeys(_steamID);
                userGames = _mapper.MapMultiple<List<UserGameEntity>>(gamesList.Games.Game, currentUser);

                // TODO: check and implement parallel add or update
                //await Parallel.ForEachAsync(games, new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount * 10 }, async (game, token) =>
                //{
                //    await new EntityRepository<GameEntity>().AddOrUpdateAsync(game);
                //});
                //await Parallel.ForEachAsync(userGames, new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount * 10 }, async (userGame, token) =>
                //{
                //    await new EntityRepository<UserGameEntity>().AddOrUpdateAsync(userGame);
                //});

                foreach (var game in games)
                {
                    await _gameRepository.AddOrUpdateAsync(game);
                }
                foreach (var userGame in userGames)
                {
                    await _userGameRepository.AddOrUpdateAsync(userGame);
                }
            }
            if (games.Count == 0)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> UpdateProfileAsync(string steamID)
        {
            Profile profile = null;
            UserEntity user = null;
            AvatarModel avatarModel = null;
            var response = await _xmlClient.SendGetRequest($"https://steamcommunity.com/profiles/{steamID}/?xml={GetRandomParameter()}");
            if (response.InnerText == XmlProfileError)
                return false;
            XmlSerializer serializer = new XmlSerializer(typeof(Profile));
            using (XmlReader reader = new XmlNodeReader(response))
            {
                profile = (Profile)serializer.Deserialize(reader);
                user = _mapper.Map<UserEntity>(profile);
                avatarModel = await GetDataFromSteamProfileAsync(profile.SteamID64);
                if (avatarModel is not null)
                {
                    user.AvatarFull = avatarModel.AvatarUrl;
                    user.AvatarFrame = avatarModel.FrameUrl;
                }
                _userRepository.AddOrUpdate(user);
            }
            if (profile?.PrivacyState != "public")
            {
                return false;
            }
            if (user is not null)
                avatarModel ??= new AvatarModel() { AvatarUrl = user.AvatarFull, FrameUrl = user.AvatarFrame };
            _steamID = steamID;
            OnAvatarUpdated?.Invoke(avatarModel);
            return true;
        }

        private async Task<AvatarModel> GetDataFromSteamProfileAsync(string steamID)
        {
            try
            {
                string profileUrl = $"https://steamcommunity.com/profiles/{steamID}";

                // Download the HTML content of the user's Steam profile page
                string htmlContent = await httpClient.GetStringAsync(profileUrl);

                // Load the HTML content into an HtmlDocument object
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(htmlContent);

                // Find the <div> element that contains the user's avatar
                HtmlNode avatarDiv = doc.DocumentNode.SelectSingleNode("//div[@class='playerAvatarAutoSizeInner']");

                if (avatarDiv != null)
                {
                    return new AvatarModel
                    {
                        AvatarUrl = avatarDiv.SelectSingleNode("img")?.GetAttributeValue("src", ""),

                        FrameUrl = avatarDiv.SelectSingleNode("//div[@class='profile_avatar_frame']")
                            ?.SelectSingleNode("img")?.GetAttributeValue("src", "")
                    };
                }
                return default;
            }
            catch (Exception e)
            {
                return default;
            }
        }

        public bool IsLogged()
        {
            return !string.IsNullOrEmpty(_steamID);
        }

        public void LoadProfile(string steamID)
        {
            Log.Logger.Information("Loading profile for SteamID: {SteamID}", steamID);
            _steamID = steamID;
        }

        public void SaveSettingsInfo()
        {
            Settings.Default.LastUpdate = DateTime.Now;
            Settings.Default.SteamID = _steamID;
            Settings.Default.Save();
            Log.Logger.Information("Settings saved {SteamID}, {LastUpdate}", _steamID, Settings.Default.LastUpdate);
        }

        private void ValidateDBRefresh()
        {
            Log.Logger.Information("Validating DB refresh, {RefreshRequired}", _refreshRequired);
            if (_refreshRequired)
            {
                _refreshRequired = false;
                _userRepository.Refresh();
            }
        }

        public UserEntity GetUser()
        {
            ValidateDBRefresh();
            return _userRepository.GetByKeys(_steamID);
        }

        public IEnumerable<GameEntity> GetUserGames()
        {
            Log.Logger.Information("Getting user games");
            return GetUser()?.UserGames?.Select(ug => ug.Game) ?? Enumerable.Empty<GameEntity>();
        }

        public void QueueAchievementsUpdate(bool onlyRecentGames = true)
        {
            Log.Logger.Information("Queueing achievements update for SteamID: {SteamID}", _steamID);
            if (string.IsNullOrEmpty(_steamID))
            {
                return;
            }

            if (onlyRecentGames)
                _gameQueueService.Add(_userRepository.GetRecentGamesToQueue(_steamID, AchievementsRecentUpdateInterval));
            else
                _gameQueueService.Add(_userRepository.GetGamesToQueue(_steamID));
        }

        public void AchievementsDataChanged()
        {
            Log.Logger.Information("Achievements data changed for SteamID: {SteamID}", _steamID);
            _refreshRequired = true;
        }

        public string GetUserId()
        {
            Log.Logger.Information("Getting user ID: {SteamID}", _steamID);
            return _steamID;
        }
    }
}
