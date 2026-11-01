using Sav.Infrastructure.Entities;

namespace Sav.Common.Services
{
    public delegate void AchievementProgressUpdatedDelegate(int totalGames, int currentGameCount, string lastGameName);

    public enum ProfileUpdateResult
    {
        Success,
        NotFound,
        Private,
        Failed
    }

    public enum GamesUpdateResult
    {
        Success,
        MissingApiKey,
        Failed
    }

    public interface ISteamService
    {
        public event AchievementProgressUpdatedDelegate OnAchievementProgressUpdated;

        public void ReportAchievementsProgress(int totalGames, int currentGameCount, string lastGameName);

        public void AchievementsDataChanged();

        public UserEntity GetUser(string? steamId = null);

        public string? GetUserId();

        public IEnumerable<GameEntity> GetUserGames(string? steamId = null);

        public void QueueAchievementsUpdate(bool force = false);

        bool IsLogged();

        Task<bool> UpdateProfileAsync(string steamID);

        Task<ProfileUpdateResult> TryUpdateProfileAsync(string steamID);

        Task<bool> UpdateGamesAsync(string steamID);

        Task<GamesUpdateResult> TryUpdateGamesAsync(string steamID);

        void LoadProfile(string steamID);

        void SaveSettingsInfo();

    }
}
