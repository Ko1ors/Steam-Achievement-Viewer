using Sav.Common.Models;

namespace Sav.Common.Interfaces
{
    public interface IAchievementsSyncTrackerService
    {
        AchievementsSyncProgress GetProgress();

        void OnGameProcessingStarted();

        void OnGameProcessed(string? gameName);

        void OnGameProcessingFailed();
    }
}
