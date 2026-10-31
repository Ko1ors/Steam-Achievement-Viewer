namespace Sav.Common.Models
{
    public class AchievementsSyncProgress
    {
        public bool IsRunning { get; set; }

        public int TotalGames { get; set; }

        public int ProcessedGames { get; set; }

        public int QueueSize { get; set; }

        public string? LastGameName { get; set; }

        public DateTime? StartedAt { get; set; }

        public DateTime? FinishedAt { get; set; }

        public int ProgressPercent => TotalGames <= 0
            ? 0
            : Math.Clamp((int)Math.Round(ProcessedGames * 100d / TotalGames), 0, 100);
    }
}
