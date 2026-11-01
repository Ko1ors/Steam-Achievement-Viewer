using System.Text.Json.Serialization;
using Sav.Infrastructure.Entities;

namespace Sav.Common.Models.DTOs
{
    public class GameSyncStatusDto
    {
        public string AppId { get; set; } = null!;

        public string Name { get; set; } = null!;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public GameSyncStatus Status { get; set; }

        public DateTime? LastSyncedAt { get; set; }

        public string? Error { get; set; }
    }
}
