using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SteamAchievementViewer.Models.SteamApi
{
    [XmlRoot(ElementName = "achievements")]
    public class Achievements
    {
        [XmlElement(ElementName = "achievement")]
        public List<Achievement> Achievement { get; set; } = new List<Achievement>();
    }

    [XmlRoot(ElementName = "achievement")]
    public class Achievement
    {
        [XmlElement(ElementName = "iconClosed")]
        public string IconClosed { get; set; }
        [XmlElement(ElementName = "iconOpen")]
        public string IconOpen { get; set; }
        [XmlElement(ElementName = "name")]
        public string Name { get; set; }
        [XmlElement(ElementName = "apiname")]
        public string Apiname { get; set; }
        [XmlElement(ElementName = "description")]
        public string Description { get; set; }
        [XmlIgnore]
        public DateTime UnlockTime { get; set; }
        [XmlAttribute(AttributeName = "closed")]
        public string Closed { get; set; }
        [XmlElement(ElementName = "percent")]
        public float Percent { get; set; }

        [XmlElement(ElementName = "unlockTimestamp")]
        public string UnlockTimestamp
        {
            get { return UnlockTime.ToString("yyyy-MM-dd HH:mm"); }
            set
            {
                if (DateTime.TryParse(value, out var res))
                    UnlockTime = res;
                else
                    UnlockTime = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(double.Parse(value));
            }
        }
    }
    public class AchievementGameInfo : Achievement
    {
        public string GameName { get; set; }
        public string GameIcon { get; set; }

        public AchievementGameInfo(Achievement achievement, Game game)
        {
            IconClosed = achievement.IconClosed;
            IconOpen = achievement.IconOpen;
            Name = achievement.Name;
            Apiname = achievement.Apiname;
            Description = achievement.Description;
            UnlockTimestamp = achievement.UnlockTimestamp;
            Closed = achievement.Closed;
            Percent = achievement.Percent;
            GameName = game.Name;
            GameIcon = game.GameIcon;
        }
    }
}
