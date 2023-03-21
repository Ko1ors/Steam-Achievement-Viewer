using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
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
            (!ug.Game.Achievements.Any() || (!string.IsNullOrEmpty(ug.HoursLast2Weeks) && ug.Game.Achievements.Any(a => a.Updated <= updateTime)))).AsEnumerable();
        }

        private IQueryable<GameEntity> GetUserGamesQueryable(string userId, bool haveAchievements = false)
        {
            return _context.UserGames.Where(ug => ug.UserId == userId && (!haveAchievements ||
            (!string.IsNullOrEmpty(ug.StatsLink) && ug.Game.Achievements.Any()))).Select(ug => ug.Game).AsQueryable();
        }

        private IQueryable<UserGameEntity> GetUserUserGamesQueryable(string userId, bool haveAchievements = false)
        {
            return _context.UserGames.Where(ug => ug.UserId == userId && (!haveAchievements ||
            (!string.IsNullOrEmpty(ug.StatsLink) && ug.Game.Achievements.Any()))).AsQueryable();
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
            return GetUserUserGamesQueryable(userId, true).SelectMany(ug => ug.UserAchievements).Where(ua => ua.UserId == userId).Count();
        }

        private IQueryable<GameEntity> GetUserIncompleteGamesQueryable(string userId)
        {
            return GetUserUserGamesQueryable(userId, true).Where(ug => ug.UserAchievements.Count < ug.Game.Achievements.Count).Select(ug => ug.Game).AsQueryable();
        }

        public IEnumerable<GameEntity> GetUserIncompleteGames(string userId, int page, int count)
        {
            return GetUserIncompleteGamesQueryable(userId).Skip((page - 1) * count).Take(count).ToList();
        }

        public IEnumerable<AchievementComposite> GetUserRarestAchievements(string userId, int page, int count)
        {
            return GetUserAchievementCompositesQueryable(userId).OrderBy(e => e.Percent).Where(e => !e.Unlocked && e.Percent != -1).Skip((page - 1) * count).Take(count).ToList();
        }

        public int GetUserTotalAchievementsCount(string userId)
        {
            return GetUserGamesQueryable(userId).Sum(g => g.Achievements.Count);
        }

        private IQueryable<CompletionGameComposite> GetUserEasiestGamesToCompleteQueryable(string userId)
        {
            return GetUserIncompleteGamesQueryable(userId).Select(g =>
                new CompletionGameComposite
                {
                    Name = g.Name,
                    GameIcon = g.GameIcon,
                    AverageAchievementsPercentage = g.Achievements.Average(a => a.Percent),
                    LowestAchievementPercentage = g.Achievements.Min(a => a.Percent)
                }).OrderByDescending(cgc => cgc.LowestAchievementPercentage);
        }

        public IEnumerable<CompletionGameComposite> GetUserEasiestGamesToComplete(string userId, int page, int count)
        {
            return GetUserEasiestGamesToCompleteQueryable(userId).Skip((page - 1) * count).Take(count).ToList();
        }

        public PagedResult<CompletionGameComposite> GetPagedUserEasiestGamesToComplete(string userId, int page, int count)
        {
            var pagedResult = new PagedResult<CompletionGameComposite>();
            var queryable = GetUserEasiestGamesToCompleteQueryable(userId);
            pagedResult.Page = page;
            pagedResult.TotalCount = queryable.Count();
            pagedResult.Items = queryable.Skip((page - 1) * count).Take(count);

            // Another way to get items and total count in one query execution
            //var queryableResult = queryable.Select(g => new { Game = g, TotalCount = queryable.Count() }).Skip((page - 1) * count).Take(count);
            //pagedResult.TotalCount = queryableResult.FirstOrDefault()?.TotalCount ?? 0;
            //pagedResult.Items = queryableResult.Select(g => g.Game);

            pagedResult.Count = pagedResult.Items.Count();
            return pagedResult;
        }

        public async Task<PagedResult<CompletionGameComposite>> GetPagedUserEasiestGamesToCompleteAsync(string userId, int page, int count)
        {
            var pagedResult = new PagedResult<CompletionGameComposite>();
            var queryable = GetUserEasiestGamesToCompleteQueryable(userId);
            pagedResult.Page = page;
            pagedResult.TotalCount = await queryable.CountAsync();
            pagedResult.Items = await queryable.Skip((page - 1) * count).Take(count).ToListAsync();

            pagedResult.Count = pagedResult.Items.Count();
            return pagedResult;
        }

        public async Task<PagedResult<CompletedGameComposite>> GetPagedUserCompletedGamesAsync(string userId, int page, int count)
        {
            var pagedResult = new PagedResult<CompletedGameComposite>();
            var queryable = GetUserUserGamesQueryable(userId, true).AsNoTracking()
                .Where(ug => ug.UserAchievements.Count == ug.Game.Achievements.Count)
            .Select(ug => new CompletedGameComposite
            {
                UserGame = ug,
                Game = ug.Game,
                Achievements = ug.Game.Achievements,
                CompletedAt = ug.UserAchievements.Max(ua => ua.UnlockTime)
            })
            .OrderByDescending(c => c.CompletedAt);

            pagedResult.Page = page;
            pagedResult.TotalCount = await queryable.CountAsync();
            pagedResult.Items = await queryable.Skip((page - 1) * count).Take(count).ToListAsync();
            return pagedResult;
        }
    }
}
