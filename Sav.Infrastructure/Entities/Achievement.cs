using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Sav.Infrastructure.Entities
{
    public class Achievement : BaseEntity
    {
        public string IconClosed { get; set; } = null!;

        public string IconOpen { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string Apiname { get; set; } = null!;

        public string Description { get; set; } = null!;

        public float Percent { get; set; }

        public string AppID { get; set; } = null!;

        public virtual Game Game { get; set; } = null!;

        public virtual ICollection<UserAchievement> UserAchievements { get; set; } = null!;
    }
}
