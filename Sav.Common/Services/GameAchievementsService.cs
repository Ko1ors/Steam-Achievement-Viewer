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
            return _userRepository.GetUserAchievementComposites(_steamService.GetUserId());
        }

        public IEnumerable<AchievementComposite> GetClosestAchievements(string steamId, int take = 25, int skip = 0)
        {
            return _userRepository.GetUserClosestAchievements(steamId, take, skip);
        }

        public IEnumerable<AchievementComposite> GetGameClosestAchievements(string steamId, string appid)
        {
            return _userRepository.GetUserGameClosestAchievements(steamId, appid);
        }

        public int GetCompletedAchievementsCount()
        {
            return _userRepository.GetUserCompletedAchievementsCount(_steamService.GetUserId());
        }

        public IEnumerable<GameEntity> GetIncompleteGames(string steamId, int take = 25, int skip = 0)
        {
            return _userRepository.GetUserIncompleteGames(steamId, take, skip);
        }

        public IEnumerable<AchievementComposite> GetLatestAchievements(int page, int count)
        {
            return _userRepository.GetUserLatestAchievements(_steamService.GetUserId(), page, count);
        }

        public IEnumerable<AchievementComposite> GetRarestAchievements(int page, int count)
        {
            return _userRepository.GetUserRarestAchievements(_steamService.GetUserId(), page, count);
        }

        public int GetTotalAchievementsCount()
        {
            return _userRepository.GetUserTotalAchievementsCount(_steamService.GetUserId());
        }

        public IEnumerable<CompletionGameComposite> GetEasiestGamesToComplete(int page = 1, int count = 100)
        {
            return _userRepository.GetUserEasiestGamesToComplete(_steamService.GetUserId(), page, count);
        }

        public PagedResult<CompletionGameComposite> GetPagedEasiestGamesToComplete(int page = 1, int count = 100)
        {
            return _userRepository.GetPagedUserEasiestGamesToComplete(_steamService.GetUserId(), page, count);
        }

        public Task<PagedResult<CompletionGameComposite>> GetPagedEasiestGamesToCompleteAsync(int page = 1, int count = 100)
        {
            return _userRepository.GetPagedUserEasiestGamesToCompleteAsync(_steamService.GetUserId(), page, count);
        }

        public async Task<PagedResult<CompletedGameComposite>> GetPagedCompletedGamesAsync(string steamId, int take = 25, int skip = 0)
        {
            return await _userRepository.GetPagedUserCompletedGamesAsync(steamId, take, skip);
        }
    }
}
