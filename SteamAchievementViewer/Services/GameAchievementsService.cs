using AutoMapper;
using Sav.Common.Models;
using Sav.Infrastructure.Entities;
using SteamAchievementViewer.Mapping;
using System.Collections.Generic;
using System.Linq;

namespace SteamAchievementViewer.Services
{
    public class GameAchievementsService : IGameAchievementsService
    {
        private readonly ISteamService _steamService;
        private readonly IMapper _mapper;

        public GameAchievementsService(ISteamService steamService, IMapper mapper)
        {
            _steamService = steamService;
            _mapper = mapper;
        }

        public IEnumerable<AchievementComposite> GetAchievementComposites()
        {
            return _steamService.GetAchievementComposites();
        }

        public IEnumerable<AchievementComposite> GetClosestAchievements(int page, int count)
        {
            return GetAchievementComposites().OrderByDescending(e => e.Percent).Where(e => !e.Unlocked).Skip((page - 1) * count).Take(count);
        }

        public IEnumerable<AchievementEntity> GetClosestAchievements(string appid)
        {
            var game = _steamService.GetUserGames().FirstOrDefault(e => e.AppID == appid);
            if (game == null)
                return null;

            return game.Achievements.Where(a => !game.UserAchievements.Any(ua => ua.Apiname == a.Apiname)).OrderByDescending(e => e.Percent);
        }

        public int GetCompletedAchievementsCount()
        {
            return _steamService.GetUserGames().Sum(g => g.UserAchievements.Count());
        }

        public IEnumerable<GameEntity> GetIncompleteGames(int page, int count)
        {
            return _steamService.GetUserGames().Where(u => u.UserAchievements.Count < u.Achievements.Count).Skip((page - 1) * count).Take(count);
        }

        public IEnumerable<AchievementComposite> GetLatestAchievements(int page, int count)
        {
            return GetAchievementComposites().OrderByDescending(e => e.UnlockTime).Where(e => e.Unlocked).Skip((page - 1) * count).Take(count);
        }

        public IEnumerable<AchievementComposite> GetRarestAchievements(int page, int count)
        {
            return GetAchievementComposites().OrderBy(e => e.Percent).Where(e => !e.Unlocked && e.Percent != -1).Skip((page - 1) * count).Take(count);
        }

        public int GetTotalAchievementsCount()
        {
            return _steamService.GetUserGames().Sum(g => g.Achievements.Count);
        }
    }
}
