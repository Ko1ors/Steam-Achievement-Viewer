using AutoMapper;
using Sav.Common.Interfaces;
using Sav.Common.Models;
using Sav.Infrastructure.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SteamAchievementViewer.Services
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

        public IEnumerable<AchievementComposite> GetClosestAchievements(int page, int count)
        {
            return _userRepository.GetUserClosestAchievements(_steamService.GetUserId(), page, count);
        }

        public IEnumerable<AchievementComposite> GetGameClosestAchievements(string appid)
        {
            return _userRepository.GetUserGameClosestAchievements(_steamService.GetUserId(), appid);
        }

        public int GetCompletedAchievementsCount()
        {
            return _userRepository.GetUserCompletedAchievementsCount(_steamService.GetUserId());
        }

        public IEnumerable<GameEntity> GetIncompleteGames(int page, int count)
        {
            return _userRepository.GetUserIncompleteGames(_steamService.GetUserId(), page, count);
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
    }
}
