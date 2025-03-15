using Sav.Infrastructure.Entities;

namespace Sav.Common.Services
{
    public delegate void AchievementProgressUpdatedDelegate(int totalGames, int currentGameCount, string lastGameName);

    public interface ISteamService
    {
        public event AchievementProgressUpdatedDelegate OnAchievementProgressUpdated;

        public void AchievementsDataChanged();

        public UserEntity GetUser();

        public string GetUserId();

        public IEnumerable<GameEntity> GetUserGames();

        public void QueueAchievementsUpdate(bool onlyRecentGames = true);

        bool IsLogged();

        Task<bool> UpdateProfileAsync(string steamID);

        Task<bool> UpdateGamesAsync(string steamID);

        void LoadProfile(string steamID);

        void SaveSettingsInfo();

    }
}
