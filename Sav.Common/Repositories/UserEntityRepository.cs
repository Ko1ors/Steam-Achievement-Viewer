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
        
        public IEnumerable<UserGameEntity> GetGamesToQueue(string userId, TimeSpan updateInterval)
        {
            var updateTime = DateTime.Now - updateInterval;
            return _context.UserGames.Where(ug => ug.UserId == userId && !string.IsNullOrEmpty(ug.StatsLink) &&
            (!ug.Game.Achievements.Any() || ug.Game.Achievements.Any(a => a.Updated <= updateTime))).AsEnumerable();
        }

        private IQueryable<GameEntity> GetUserGamesQueryable(string userId, bool haveAchievements = false)
        {
            return _context.UserGames.Where(ug => ug.UserId == userId && (!haveAchievements ||
            (!string.IsNullOrEmpty(ug.StatsLink) && ug.Game.Achievements.Any()))).Select(ug => ug.Game).AsQueryable();
        }

        public IEnumerable<AchievementComposite> GetUserAchievementComposites(string userId)
        {
            var entityCompositesQuery = from query in GetUserGamesQueryable(userId, true)
                         from a in query.Achievements
                         join ua in _context.UserAchievements
                         on new { a.AppID, a.Apiname } equals new { ua.AppID, ua.Apiname } into uaj
                         from ua in uaj.DefaultIfEmpty()
                         select new EntityComposite { Game = query, Achievement = a, UserAchievement = ua };
            return entityCompositesQuery.ProjectTo<AchievementComposite>(_mapper.ConfigurationProvider);
        }
    }
}
