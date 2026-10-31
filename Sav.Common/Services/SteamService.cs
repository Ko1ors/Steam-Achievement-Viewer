using AutoMapper;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Sav.Common.Interfaces;
using Sav.Common.Logs;
using Sav.Common.Mapping;
using Sav.Common.Models;
using Sav.Common.Models.SteamApi;
using Sav.Infrastructure.Entities;
using Serilog.Core;
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

namespace Sav.Common.Services
{
    public class SteamService : ISteamService
    {
        private const string XmlProfileError = "The specified profile could not be found.";
        private static readonly TimeSpan AchievementsUpdateInterval = TimeSpan.FromHours(24);
        private static readonly TimeSpan AchievementsRecentUpdateInterval = TimeSpan.FromMinutes(5);

        private readonly Random _random;
        private readonly IClientService<XmlDocument> _xmlClient;
        private readonly ISteamApiClientService _steamApiClientService;
        private readonly IQueueService<UserGameEntity> _gameQueueService;
        private readonly IMapper _mapper;

        private readonly IUserEntityRepository _userRepository;
        private readonly IEntityRepository<GameEntity> _gameRepository;
        private readonly IEntityRepository<UserGameEntity> _userGameRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ISettingsService _settingsService;

        public event AchievementProgressUpdatedDelegate OnAchievementProgressUpdated;

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

        public SteamService(IClientService<XmlDocument> xmlClient, ISteamApiClientService steamApiClientService, IQueueService<UserGameEntity> gameQueueService, IMapper mapper,
            IUserEntityRepository userRepository, IGameEntityRepository gameRepository,
            IEntityRepository<UserGameEntity> userGameRepository, ICurrentUserService currentUserService,
            ISettingsService settingsService)
        {
            _xmlClient = xmlClient;
            _steamApiClientService = steamApiClientService;
            _gameQueueService = gameQueueService;
            _mapper = mapper;
            _userRepository = userRepository;
            _gameRepository = gameRepository;
            _userGameRepository = userGameRepository;
            _currentUserService = currentUserService;
            _settingsService = settingsService;

            _random = new Random();
            _refreshRequired = false;
        }

        //private bool Start()
        //{
        //    Log.Logger.Information("SteamService started");

        //    if (Settings.Default.SteamID == "-1")
        //    {
        //        Log.Logger.Information("SteamID is not set");
        //        return false;
        //    }

        //    LoadProfile(Settings.Default.SteamID);

        //    Log.Logger.Information("SteamService started successfully");
        //    return true;
        //}

        private int GetRandomParameter() => _random.Next(0, 100000);

        public async Task<bool> UpdateGamesAsync(string steamID)
        {
            return await TryUpdateGamesAsync(steamID) == GamesUpdateResult.Success;
        }

        public async Task<GamesUpdateResult> TryUpdateGamesAsync(string steamID)
        {
            try
            {
                List<GameEntity> games;
                List<UserGameEntity> userGames;

                var currentUser = _userRepository.GetByKeys(steamID);
                var response = await _xmlClient.SendGetRequest($"https://steamcommunity.com/profiles/{steamID}/games?tab=all&xml={GetRandomParameter()}");

                // Steam no longer serves the public games XML feed, so fall back to the Steam Web API.
                if (response.InnerText == XmlProfileError || string.IsNullOrEmpty(response.InnerText))
                {
                    var steamApiKey = _settingsService.Get().SteamApiKey;
                    if (string.IsNullOrWhiteSpace(steamApiKey))
                    {
                        Log.Logger.Warning("Steam Web API key is not set, the game list cannot be retrieved for {SteamId}", steamID);
                        return GamesUpdateResult.MissingApiKey;
                    }

                    var ownedGames = await _steamApiClientService.GetOwnedGamesAsync(steamID, steamApiKey);
                    games = _mapper.Map<List<GameEntity>>(ownedGames);
                    userGames = _mapper.MapMultiple<List<UserGameEntity>>(ownedGames, currentUser);
                }
                else
                {
                    var serializer = new XmlSerializer(typeof(GamesList));
                    using (XmlReader reader = new XmlNodeReader(response))
                    {
                        var gamesList = (GamesList)serializer.Deserialize(reader);
                        games = _mapper.Map<List<GameEntity>>(gamesList.Games.Game);
                        userGames = _mapper.MapMultiple<List<UserGameEntity>>(gamesList.Games.Game, currentUser);
                    }
                }

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

                if (games.Count == 0)
                {
                    Log.Logger.Warning("No games were returned for {SteamId}", steamID);
                    return GamesUpdateResult.Failed;
                }

                return GamesUpdateResult.Success;
            }
            catch (Exception e)
            {
                Log.Logger.Error(e, "Failed to update the game list for {SteamId}", steamID);
                return GamesUpdateResult.Failed;
            }
        }

        public async Task<bool> UpdateProfileAsync(string steamID)
        {
            return await TryUpdateProfileAsync(steamID) == ProfileUpdateResult.Success;
        }

        public async Task<ProfileUpdateResult> TryUpdateProfileAsync(string steamID)
        {
            try
            {
                var response = await _xmlClient.SendGetRequest($"https://steamcommunity.com/profiles/{steamID}/?xml={GetRandomParameter()}");
                if (response.InnerText == XmlProfileError)
                    return ProfileUpdateResult.NotFound;
                if (string.IsNullOrEmpty(response.InnerText))
                    return ProfileUpdateResult.Failed;

                Profile profile;
                XmlSerializer serializer = new XmlSerializer(typeof(Profile));
                using (XmlReader reader = new XmlNodeReader(response))
                {
                    profile = (Profile)serializer.Deserialize(reader);
                }

                if (profile.PrivacyState != "public")
                    return ProfileUpdateResult.Private;

                var user = _mapper.Map<UserEntity>(profile);
                var avatarModel = await GetDataFromSteamProfileAsync(profile.SteamID64);
                if (!string.IsNullOrWhiteSpace(avatarModel?.AvatarUrl))
                {
                    user.AvatarFull = avatarModel.AvatarUrl;
                    user.AvatarFrame = avatarModel.FrameUrl;
                }
                _userRepository.AddOrUpdate(user);

                _currentUserService.SetCurrentUser(steamID);
                return ProfileUpdateResult.Success;
            }
            catch (Exception e)
            {
                Log.Logger.Error(e, "Failed to update the profile {SteamID}", steamID);
                return ProfileUpdateResult.Failed;
            }
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

                if (avatarDiv is null)
                    return default;

                return new AvatarModel
                {
                    AvatarUrl = GetImageUrl(avatarDiv.SelectSingleNode("picture/img")),
                    FrameUrl = GetImageUrl(avatarDiv.SelectSingleNode("div[@class='profile_avatar_frame']//img"))
                };
            }
            catch (Exception e)
            {
                return default;
            }
        }

        private static string GetImageUrl(HtmlNode image)
        {
            // Steam serves the avatar through srcset only, while the frame keeps a regular src attribute
            var url = image?.GetAttributeValue("srcset", "");
            if (string.IsNullOrWhiteSpace(url))
                url = image?.GetAttributeValue("src", "");

            return string.IsNullOrWhiteSpace(url) ? string.Empty : url.Split(' ')[0];
        }

        public bool IsLogged()
        {
            return _currentUserService.IsAuthenticated;
        }

        public void LoadProfile(string steamID)
        {
            Log.Logger.Information("Loading profile for SteamID: {SteamID}", steamID);
            _currentUserService.SetCurrentUser(steamID);
        }

        public void SaveSettingsInfo()
        {
            //Settings.Default.LastUpdate = DateTime.Now;
            //Settings.Default.SteamID = _steamID;
            //Settings.Default.Save();
            //Log.Logger.Information("Settings saved {SteamID}, {LastUpdate}", _steamID, Settings.Default.LastUpdate);
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

        public UserEntity GetUser(string? steamId = null)
        {
            ValidateDBRefresh();
            var userId = steamId ?? _currentUserService.SteamId;
            if (string.IsNullOrEmpty(userId))
                return null;

            return _userRepository.GetByKeys(userId);
        }

        public IEnumerable<GameEntity> GetUserGames(string? steamId = null)
        {
            Log.Logger.Information("Getting user games");
            return GetUser(steamId)?.UserGames?.Select(ug => ug.Game) ?? Enumerable.Empty<GameEntity>();
        }

        public void QueueAchievementsUpdate(bool onlyRecentGames = true)
        {
            var steamId = _currentUserService.SteamId;
            Log.Logger.Information("Queueing achievements update for SteamID: {SteamID}", steamId);
            if (string.IsNullOrEmpty(steamId))
            {
                return;
            }

            if (onlyRecentGames)
                _gameQueueService.Add(_userRepository.GetRecentGamesToQueue(steamId, AchievementsRecentUpdateInterval));
            else
                _gameQueueService.Add(_userRepository.GetGamesToQueue(steamId));
        }

        public void AchievementsDataChanged()
        {
            Log.Logger.Information("Achievements data changed for SteamID: {SteamID}", _currentUserService.SteamId);
            _refreshRequired = true;
        }

        public string? GetUserId()
        {
            var steamId = _currentUserService.SteamId;
            Log.Logger.Information("Getting user ID: {SteamID}", steamId);
            return steamId;
        }
    }
}
