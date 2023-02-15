using AutoMapper;
using AutoMapper.QueryableExtensions;
using Sav.Common.Interfaces;
using Sav.Common.Models;
using Sav.Infrastructure.Entities;

namespace Sav.Common.Repositories
{
    public class UserEntityRepository : EntityRepository<UserEntity>, IUserEntityRepository
    {
        private readonly IMapper _mapper;

        public UserEntityRepository(IMapper mapper) : base()
        {
            _mapper = mapper;
        }

        public IEnumerable<UserGameEntity> GetGamesToQueue(string userId)
        {
            return _context.UserGames.Where(ug => ug.UserId == userId && !string.IsNullOrEmpty(ug.StatsLink)).AsEnumerable();
        }

        public IEnumerable<UserGameEntity> GetRecentGamesToQueue(string userId, TimeSpan updateInterval)
        {
            var updateTime = DateTime.Now - updateInterval;
            return _context.UserGames.Where(ug => ug.UserId == userId && !string.IsNullOrEmpty(ug.StatsLink) &&
            (!ug.Game.Achievements.Any() || (!string.IsNullOrEmpty(ug.Game.HoursLast2Weeks) && ug.Game.Achievements.Any(a => a.Updated <= updateTime)))).AsEnumerable();
        }

        private IQueryable<GameEntity> GetUserGamesQueryable(string userId, bool haveAchievements = false)
        {
            return _context.UserGames.Where(ug => ug.UserId == userId && (!haveAchievements ||
            (!string.IsNullOrEmpty(ug.StatsLink) && ug.Game.Achievements.Any()))).Select(ug => ug.Game).AsQueryable();
        }

        public IEnumerable<AchievementComposite> GetUserAchievementComposites(string userId)
        {
            return GetUserAchievementCompositesQueryable(userId).AsEnumerable();
        }

        private IQueryable<AchievementComposite> GetUserAchievementCompositesQueryable(string userId)
        {
            var entityCompositesQuery = from query in GetUserGamesQueryable(userId, true)
                                        from a in query.Achievements
                                        join ua in _context.UserAchievements
                                        on new { a.AppID, a.Apiname } equals new { ua.AppID, ua.Apiname } into uaj
                                        from ua in uaj.DefaultIfEmpty()
                                        where ua.UserId == userId || ua == null 
                                        select new EntityComposite { Game = query, Achievement = a, UserAchievement = ua };
            return entityCompositesQuery.ProjectTo<AchievementComposite>(_mapper.ConfigurationProvider);
        }

        public IEnumerable<AchievementComposite> GetUserLatestAchievements(string userId, int page, int count)
        {
            return GetUserAchievementCompositesQueryable(userId).OrderByDescending(e => e.UnlockTime).Where(e => e.Unlocked).Skip((page - 1) * count).Take(count).ToList();
        }

        public IEnumerable<AchievementComposite> GetUserClosestAchievements(string userId, int page, int count)
        {
            return GetUserAchievementCompositesQueryable(userId).OrderByDescending(e => e.Percent).Where(e => !e.Unlocked).Skip((page - 1) * count).Take(count).ToList();
        }

        public IEnumerable<AchievementComposite> GetUserGameClosestAchievements(string userId, string appId)
        {
            return GetUserAchievementCompositesQueryable(userId).Where(ac => ac.AppID == appId && !ac.Unlocked).OrderByDescending(e => e.Percent).ToList();
        }

        public int GetUserCompletedAchievementsCount(string userId)
        {
            return GetUserGamesQueryable(userId, true).SelectMany(g => g.UserAchievements).Where(ua => ua.UserId == userId).Count();
        }

        public IEnumerable<GameEntity> GetUserIncompleteGames(string userId, int page, int count)
        {
            return GetUserGamesQueryable(userId, true).Where(u => u.UserAchievements.Count < u.Achievements.Count).Skip((page - 1) * count).Take(count).ToList();
        }

        public IEnumerable<AchievementComposite> GetUserRarestAchievements(string userId, int page, int count)
        {
            return GetUserAchievementCompositesQueryable(userId).OrderBy(e => e.Percent).Where(e => !e.Unlocked && e.Percent != -1).Skip((page - 1) * count).Take(count).ToList();
        }

        public int GetUserTotalAchievementsCount(string userId)
        {
            return GetUserGamesQueryable(userId).Sum(g => g.Achievements.Count);
        }
    }
}
