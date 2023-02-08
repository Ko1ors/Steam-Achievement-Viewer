using System.ComponentModel.DataAnnotations.Schema;

namespace Sav.Infrastructure.Entities
{
    [Table("UserGames")]
    public class UserGameEntity : BaseEntity
    {
        public string UserId { get; set; } = null!;

        public string AppID { get; set; } = null!;

        public string StatsLink { get; set; } = null!;

        public virtual UserEntity User { get; set; } = null!;

        public virtual GameEntity Game { get; set; } = null!;

        public override object[] GetKeys()
        {
            return new object[] { UserId, AppID };
        }
    }
}
