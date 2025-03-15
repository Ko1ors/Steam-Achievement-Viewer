using Sav.Infrastructure.Entities;

namespace Sav.Common.Models.DTOs
{
    public class GameDto
    {
        public string AppID { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string? GameIcon { get; set; } = null!;

        public string Logo { get; set; } = null!;

        public string? GameLogoSmall { get; set; }

        public string StoreLink { get; set; } = null!;

        public string? GlobalStatsLink { get; set; }


        public GameDto(GameEntity game)
        {
            AppID = game.AppID;
            Name = game.Name;
            GameIcon = game.GameIcon;
            Logo = game.Logo;
            GameLogoSmall = game.GameLogoSmall;
            StoreLink = game.StoreLink;
            GlobalStatsLink = game.GlobalStatsLink;
        }
    }
}
