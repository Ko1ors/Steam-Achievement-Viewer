namespace Sav.Common.Models.DTOs
{
    public class GameSyncStatusPageDto
    {
        public int Page { get; set; }

        public int Count { get; set; }

        public int TotalCount { get; set; }

        public int SyncedCount { get; set; }

        public int FailedCount { get; set; }

        public int SyncingCount { get; set; }

        public int NotSyncedCount { get; set; }

        public bool IsRunning { get; set; }

        public int QueueSize { get; set; }

        public IEnumerable<GameSyncStatusDto> Items { get; set; } = Array.Empty<GameSyncStatusDto>();
    }
}
