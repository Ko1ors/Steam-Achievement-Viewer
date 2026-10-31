using AutoMapper;
using Sav.Common.Interfaces;
using Sav.Common.Models;
using Sav.Infrastructure.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sav.Common.Services
{
    public class GameAchievementsService : IGameAchievementsService
    {
        private readonly ISteamService _steamService;
        private readonly IUserEntityRepository _userRepository;
        private readonly IMapper _mapper;

        public GameAchievementsService(ISteamService steamService, IUserEntityRepository userRepository, IMapper mapper)
        {
            _steamService = steamService;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public IEnumerable<AchievementComposite> GetAchievementComposites()
        {
            return _userRepository.GetUserAchievementComposites(_steamService.GetUserId() ?? string.Empty);
        }

        public IEnumerable<AchievementComposite> GetClosestAchievements(string? steamId, int take = 25, int skip = 0)
        {
            return _userRepository.GetUserClosestAchievements(ResolveSteamId(steamId), take, skip);
        }

        public IEnumerable<AchievementComposite> GetGameClosestAchievements(string? steamId, string appid)
        {
            return _userRepository.GetUserGameClosestAchievements(ResolveSteamId(steamId), appid);
        }

        public int GetCompletedAchievementsCount(string? steamId = null)
        {
            return _userRepository.GetUserCompletedAchievementsCount(ResolveSteamId(steamId));
        }

        public IEnumerable<GameEntity> GetIncompleteGames(string? steamId, int take = 25, int skip = 0)
        {
            return _userRepository.GetUserIncompleteGames(ResolveSteamId(steamId), take, skip);
        }

        public IEnumerable<AchievementComposite> GetLatestAchievements(int page, int count, string? steamId = null)
        {
            return _userRepository.GetUserLatestAchievements(ResolveSteamId(steamId), page, count);
        }

        public IEnumerable<AchievementComposite> GetRarestAchievements(int page, int count, string? steamId = null)
        {
            return _userRepository.GetUserRarestAchievements(ResolveSteamId(steamId), page, count);
        }

        public int GetTotalAchievementsCount(string? steamId = null)
        {
            return _userRepository.GetUserTotalAchievementsCount(ResolveSteamId(steamId));
        }

        public IEnumerable<CompletionGameComposite> GetEasiestGamesToComplete(string? steamId, int page = 1, int count = 100)
        {
            return _userRepository.GetUserEasiestGamesToComplete(ResolveSteamId(steamId), page, count);
        }

        public Task<PagedResult<CompletionGameComposite>> GetPagedEasiestGamesToCompleteAsync(string? steamId, int page = 1, int count = 100)
        {
            return _userRepository.GetPagedUserEasiestGamesToCompleteAsync(ResolveSteamId(steamId), page, count);
        }

        public async Task<PagedResult<CompletedGameComposite>> GetPagedCompletedGamesAsync(string? steamId, int take = 25, int skip = 0)
        {
            return await _userRepository.GetPagedUserCompletedGamesAsync(ResolveSteamId(steamId), take, skip);
        }

        private string ResolveSteamId(string? steamId) => steamId ?? _steamService.GetUserId() ?? string.Empty;
    }
}
