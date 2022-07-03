using SteamAchievementViewer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SteamAchievementViewer.Services
{
    public delegate void AchievementProgressUpdatedDelegate(int totalGames, int currentGameCount, string lastGameName);

    public interface ISteamService
    {
        public event AchievementProgressUpdatedDelegate OnAchievementProgressUpdated;

        public Profile Profile { get; }

        public GamesList GamesList { get; }

        bool Start();

        bool IsLogged();

        Task<bool> GetProfileAsync(string steamID);

        Task<bool> GetGamesAsync(string steamID);

        Task<bool> GetAchievementsAsync(List<Game> games);

        Task<bool> GetAchievementsParallelAsync(List<Game> games);

        Task<Achievements> GetGlobalAchievementPercentagesAsync(string appid);

        void SaveGames();

        void SaveProfile();

        void LoadGames();

        void LoadProfile(string steamID);

        void SaveSettingsInfo();

    }
}
