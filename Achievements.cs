using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace SteamAchievementViewer
{
	[XmlRoot(ElementName= "achievements")]
	public class Achievements
	{
		[XmlElement(ElementName = "achievement")]
		public List<Achievement> Achievement { get; set; }
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
		private DateTime unlockTime { get; set; }
		[XmlAttribute(AttributeName = "closed")]
		public string Closed { get; set; }
		[XmlElement(ElementName = "percent")]
		public float Percent { get; set; }

		[XmlElement(ElementName = "unlockTimestamp")]
		public string UnlockTime
        {
            get { return unlockTime.ToString("yyyy-MM-dd HH:mm"); }
            set { if (DateTime.TryParse(value, out var res))
					unlockTime = res;
				else
					unlockTime = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Double.Parse(value));
			}
        }
	}
	public class AchievementWithGameInfo : Achievement
	{
		public string GameName { get; set; }
		public string GameIcon { get; set; }

		public AchievementWithGameInfo(Achievement a)
        {
			IconClosed = a.IconClosed;
			IconOpen = a.IconOpen;
			Name = a.Name;
			Apiname = a.Apiname;
			Description = a.Description;
			UnlockTime = a.UnlockTime;
			Closed = a.Closed;
			Percent = a.Percent;
        }
	}
}
