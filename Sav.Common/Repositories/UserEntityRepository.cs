using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Sav.Common.Interfaces;
using Sav.Common.Logs;
using Sav.Common.Models;
using Sav.Common.Models.DTOs;
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

        public IEnumerable<UserGameEntity> GetGamesToQueue(string userId, TimeSpan resyncInterval, TimeSpan failedSyncRetryInterval)
        {
            var resyncCutoff = DateTime.Now - resyncInterval;
            var retryCutoff = DateTime.Now - failedSyncRetryInterval;

            Log.Logger.Information("Getting games to queue for user {UserId}", userId);

            // A game is queued when it was never synced, when the last attempt failed, or when it was played
            // after the last successful sync - the latter at most once per resync interval. The retry cutoff
            // keeps a game from being queued again while its previous attempt is still recent.
            var userGames = _context.UserGames
                .Where(ug => ug.UserId == userId && !string.IsNullOrEmpty(ug.StatsLink))
                .Where(ug => ug.SyncStatus != GameSyncStatus.Syncing)
                .Where(ug => ug.LastSyncAttemptedAt == null || ug.LastSyncAttemptedAt < retryCutoff);

            return userGames
                .Where(ug => ug.LastSyncedAt == null
                    || ug.SyncStatus == GameSyncStatus.Failed
                    || (ug.LastSyncedAt < resyncCutoff && ug.LastPlayedAt != null && ug.LastPlayedAt > ug.LastSyncedAt))
                .OrderByDescending(ug => ug.LastPlayedAt)
                .AsEnumerable();
        }

        public async Task<GameSyncStatusPageDto> GetGameSyncStatusAsync(string userId, int page, int count, string? search, string? status)
        {
            Log.Logger.Information("Getting game sync status for user {UserId}", userId);

            page = Math.Max(1, page);
            count = Math.Clamp(count, 1, 200);

            var userGames = _context.UserGames.Where(ug => ug.UserId == userId);

            var syncedCount = await userGames.CountAsync(ug => ug.SyncStatus == GameSyncStatus.Synced);
            var failedCount = await userGames.CountAsync(ug => ug.SyncStatus == GameSyncStatus.Failed);
            var syncingCount = await userGames.CountAsync(ug => ug.SyncStatus == GameSyncStatus.Syncing);
            var notSyncedCount = await userGames.CountAsync(ug => ug.SyncStatus == GameSyncStatus.NotSynced);

            var gamesQuery = ApplySyncStatusFilter(userGames, status);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var pattern = $"%{search.Trim().Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_")}%";
                gamesQuery = gamesQuery.Where(ug => EF.Functions.Like(ug.Game.Name, pattern, "\\"));
            }

            var totalCount = await gamesQuery.CountAsync();

            var items = await gamesQuery
                .OrderBy(ug => ug.SyncStatus == GameSyncStatus.NotSynced ? 1 : 0)
                .ThenByDescending(ug => ug.LastSyncedAt)
                .ThenBy(ug => ug.Game.Name)
                .Skip((page - 1) * count)
                .Take(count)
                .Select(ug => new GameSyncStatusDto
                {
                    AppId = ug.AppID,
                    Name = ug.Game.Name,
                    Status = ug.SyncStatus,
                    LastSyncedAt = ug.LastSyncedAt,
                    Error = ug.LastSyncError,
                })
                .ToListAsync();

            return new GameSyncStatusPageDto
            {
                Page = page,
                Count = count,
                TotalCount = totalCount,
                SyncedCount = syncedCount,
                FailedCount = failedCount,
                SyncingCount = syncingCount,
                NotSyncedCount = notSyncedCount,
                Items = items,
            };
        }

        public void SetGameSyncStatus(string userId, string appId, GameSyncStatus status, string? error)
        {
            var userGame = _context.UserGames.FirstOrDefault(ug => ug.UserId == userId && ug.AppID == appId);
            if (userGame is null)
                return;

            userGame.SyncStatus = status;
            userGame.LastSyncError = error;
            userGame.Updated = DateTime.Now;
            userGame.LastSyncAttemptedAt = userGame.Updated;

            if (status == GameSyncStatus.Synced)
                userGame.LastSyncedAt = userGame.Updated;

            _context.SaveChanges();
        }

        public void ResetInterruptedSyncs()
        {
            var interrupted = _context.UserGames.Where(ug => ug.SyncStatus == GameSyncStatus.Syncing).ToList();
            if (interrupted.Count == 0)
                return;

            foreach (var userGame in interrupted)
            {
                userGame.SyncStatus = GameSyncStatus.Failed;
                userGame.LastSyncError = "The sync was interrupted before it finished.";
                userGame.Updated = DateTime.Now;
            }

            _context.SaveChanges();
            Log.Logger.Warning("Marked {Count} interrupted game syncs as failed", interrupted.Count);
        }

        private static IQueryable<UserGameEntity> ApplySyncStatusFilter(IQueryable<UserGameEntity> query, string? status)
        {
            if (Enum.TryParse<GameSyncStatus>(status, true, out var parsedStatus) && Enum.IsDefined(parsedStatus))
                return query.Where(ug => ug.SyncStatus == parsedStatus);

            return query;
        }

        private IQueryable<GameEntity> GetUserGamesQueryable(string userId, bool haveAchievements = false)
        {
            Log.Logger.Information("Getting games for user {UserId}", userId);
            return _context.UserGames.Where(ug => ug.UserId == userId && (!haveAchievements ||
            (!string.IsNullOrEmpty(ug.StatsLink) && ug.Game.Achievements.Any()))).Select(ug => ug.Game).AsQueryable();
        }

        private IQueryable<UserGameEntity> GetUserUserGamesQueryable(string userId, bool haveAchievements = false)
        {
            Log.Logger.Information("Getting user games for user {UserId}", userId);
            return _context.UserGames.Where(ug => ug.UserId == userId && (!haveAchievements ||
            (!string.IsNullOrEmpty(ug.StatsLink) && ug.Game.Achievements.Any()))).AsQueryable();
        }

        public IEnumerable<AchievementComposite> GetUserAchievementComposites(string userId)
        {
            return GetUserAchievementCompositesQueryable(userId).AsEnumerable();
        }

        private IQueryable<AchievementComposite> GetUserAchievementCompositesQueryable(string userId)
        {
            Log.Logger.Information("Getting achievement composites for user {UserId}", userId);
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
            Log.Logger.Information("Getting latest achievements for user {UserId}, {Page}, {Count}", userId, page, count);
            return GetUserAchievementCompositesQueryable(userId).OrderByDescending(e => e.UnlockTime).Where(e => e.Unlocked).Skip((page - 1) * count).Take(count).ToList();
        }

        public IEnumerable<AchievementComposite> GetUserClosestAchievements(string userId, int take, int skip)
        {
            Log.Logger.Information("Getting closest achievements for user {UserId}, {Take}, {Skip}", userId, take, skip);
            return GetUserAchievementCompositesQueryable(userId).OrderByDescending(e => e.Percent).Where(e => !e.Unlocked).Skip(skip).Take(take).ToList();
        }

        public IEnumerable<AchievementComposite> GetUserGameClosestAchievements(string userId, string appId)
        {
            Log.Logger.Information("Getting closest achievements for user {UserId}, {AppId}", userId, appId);
            return GetUserAchievementCompositesQueryable(userId).Where(ac => ac.AppID == appId && !ac.Unlocked).OrderByDescending(e => e.Percent).ToList();
        }

        public int GetUserCompletedAchievementsCount(string userId)
        {
            Log.Logger.Information("Getting completed achievements count for user {UserId}", userId);
            return GetUserUserGamesQueryable(userId, true).SelectMany(ug => ug.UserAchievements).Where(ua => ua.UserId == userId).Count();
        }

        private IQueryable<GameEntity> GetUserIncompleteGamesQueryable(string userId)
        {
            Log.Logger.Information("Getting incomplete games for user {UserId}", userId);
            return GetUserUserGamesQueryable(userId, true).Where(ug => ug.UserAchievements.Count < ug.Game.Achievements.Count).Select(ug => ug.Game).AsQueryable();
        }

        public IEnumerable<GameEntity> GetUserIncompleteGames(string userId, int take, int skip)
        {
            Log.Logger.Information("Getting incomplete games for user {UserId}, {Take}, {Skip}", userId, take, skip);
            return GetUserIncompleteGamesQueryable(userId).Skip(skip).Take(take).ToList();
        }

        public IEnumerable<AchievementComposite> GetUserRarestAchievements(string userId, int page, int count)
        {
            Log.Logger.Information("Getting rarest achievements for user {UserId}, {Page}, {Count}", userId, page, count);
            return GetUserAchievementCompositesQueryable(userId).OrderBy(e => e.Percent).Where(e => !e.Unlocked && e.Percent != -1).Skip((page - 1) * count).Take(count).ToList();
        }

        public int GetUserTotalAchievementsCount(string userId)
        {
            Log.Logger.Information("Getting total achievements count for user {UserId}", userId);
            return GetUserGamesQueryable(userId).Sum(g => g.Achievements.Count);
        }

        private IQueryable<CompletionGameComposite> GetUserEasiestGamesToCompleteQueryable(string userId)
        {
            Log.Logger.Information("Getting easiest games to complete for user {UserId}", userId);
            return GetUserIncompleteGamesQueryable(userId).Select(g =>
                new CompletionGameComposite
                {
                    AppID = g.AppID,
                    Name = g.Name,
                    GameIcon = g.GameIcon,
                    StoreLink = g.StoreLink,
                    GlobalStatsLink = g.GlobalStatsLink,
                    AverageAchievementsPercentage = g.Achievements.Average(a => a.Percent),
                    LowestAchievementPercentage = g.Achievements.Min(a => a.Percent)
                }).OrderByDescending(cgc => cgc.LowestAchievementPercentage);
        }

        public IEnumerable<CompletionGameComposite> GetUserEasiestGamesToComplete(string userId, int page, int count)
        {
            Log.Logger.Information("Getting easiest games to complete for user {UserId}, {Page}, {Count}", userId, page, count);
            return GetUserEasiestGamesToCompleteQueryable(userId).Skip((page - 1) * count).Take(count).ToList();
        }

        public async Task<PagedResult<CompletionGameComposite>> GetPagedUserEasiestGamesToCompleteAsync(string userId, int page, int count)
        {
            Log.Logger.Information("Getting paged easiest games to complete for user {UserId}, {Page}, {Count}", userId, page, count);
            var pagedResult = new PagedResult<CompletionGameComposite>();
            var queryable = GetUserEasiestGamesToCompleteQueryable(userId);
            pagedResult.Page = page;
            pagedResult.TotalCount = await queryable.CountAsync();
            pagedResult.Items = await queryable.Skip((page - 1) * count).Take(count).ToListAsync();

            pagedResult.Count = pagedResult.Items.Count();
            Log.Logger.Information("Easiest games to complete count: {Count}", pagedResult.Count);
            return pagedResult;
        }

        public async Task<PagedResult<CompletedGameComposite>> GetPagedUserCompletedGamesAsync(string userId, int take, int skip)
        {
            Log.Logger.Information("Getting paged completed games for user {UserId}, {Take}, {Skip}", userId, take, skip);
            var pagedResult = new PagedResult<CompletedGameComposite>();
            var queryable = GetUserUserGamesQueryable(userId, true).AsNoTracking()
                .Where(ug => ug.UserAchievements.Count == ug.Game.Achievements.Count)
            .OrderByDescending(ug => ug.UserAchievements.Max(ua => ua.UnlockTime));

            pagedResult.Page = skip > 0 ? skip / take + 1 : 1;
            pagedResult.TotalCount = await queryable.CountAsync();
            pagedResult.Items = await queryable.Skip(skip).Take(take).Select(ug => new CompletedGameComposite(ug)).ToListAsync();
            Log.Logger.Information("Completed games count: {Count}", pagedResult.TotalCount);
            return pagedResult;
        }
    }
}
