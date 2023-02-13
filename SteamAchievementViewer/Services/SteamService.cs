using AutoMapper;
using Sav.Common.Interfaces;
using Sav.Infrastructure.Entities;
using SteamAchievementViewer.Mapping;
using SteamAchievementViewer.Models.SteamApi;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public SteamService(IClientService<XmlDocument> xmlClient, IQueueService<UserGameEntity> gameQueueService, IMapper mapper,
            IUserEntityRepository userRepository, IEntityRepository<GameEntity> gameRepository,
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
            if (Settings.Default.SteamID == "-1")
                return false;

            LoadProfile(Settings.Default.SteamID);

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
            QueueAchievementsUpdate();
            
            return true;
        }

        public async Task<bool> UpdateProfileAsync(string steamID)
        {
            Profile profile = null;
            var response = await _xmlClient.SendGetRequest($"https://steamcommunity.com/profiles/{steamID}/?xml={GetRandomParameter()}");
            if (response.InnerText == XmlProfileError)
                return false;
            XmlSerializer serializer = new XmlSerializer(typeof(Profile));
            using (XmlReader reader = new XmlNodeReader(response))
            {
                profile = (Profile)serializer.Deserialize(reader);
                var user = _mapper.Map<UserEntity>(profile);
                _userRepository.AddOrUpdate(user);
            }
            if (profile?.PrivacyState != "public")
            {
                return false;
            }
            OnAvatarUpdated?.Invoke(profile.AvatarFull);
            return true;
        }

        public bool IsLogged()
        {
            return !string.IsNullOrEmpty(_steamID);
        }

        public void LoadProfile(string steamID)
        {
            _steamID = steamID;
        }

        public void SaveSettingsInfo()
        {
            Settings.Default.LastUpdate = DateTime.Now;
            Settings.Default.Save();
        }

        private void ValidateDBRefresh()
        {
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
            return GetUser()?.UserGames?.Select(ug => ug.Game) ?? Enumerable.Empty<GameEntity>();
        }

        public void QueueAchievementsUpdate()
        {
            if (string.IsNullOrEmpty(_steamID))
            {
                return;
            }

            _gameQueueService.Add(_userRepository.GetRecentGamesToQueue(_steamID, AchievementsRecentUpdateInterval));
        }

        public void AchievementsDataChanged()
        {
            _refreshRequired = true;
        }

        public string GetUserId()
        {
            return _steamID;
        }
    }
}
