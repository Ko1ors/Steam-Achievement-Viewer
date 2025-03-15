using Sav.Infrastructure.Entities;

namespace Sav.Common.Models.DTOs
{
    public class UserGameDto
    {
        public string UserId { get; set; } = null!;

        public string AppID { get; set; } = null!;

        public string? StatsLink { get; set; }

        public string? HoursLast2Weeks { get; set; }

        public string? HoursOnRecord { get; set; }

        public UserGameDto(UserGameEntity userGame)
        {
            UserId = userGame.UserId;
            AppID = userGame.AppID;
            StatsLink = userGame.StatsLink;
            HoursLast2Weeks = userGame.HoursLast2Weeks;
            HoursOnRecord = userGame.HoursOnRecord;
        }
    }
}
