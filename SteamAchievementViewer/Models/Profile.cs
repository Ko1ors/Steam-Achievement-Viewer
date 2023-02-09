using System.Collections.Generic;
using System.Xml.Serialization;

namespace SteamAchievementViewer.Models
{

    [XmlRoot(ElementName = "profile")]
    public class Profile
    {
        [XmlElement(ElementName = "steamID64")]
        public string SteamID64 { get; set; }
        [XmlElement(ElementName = "steamID")]
        public string SteamID { get; set; }
        [XmlElement(ElementName = "onlineState")]
        public string OnlineState { get; set; }
        [XmlElement(ElementName = "stateMessage")]
        public string StateMessage { get; set; }
        [XmlElement(ElementName = "privacyState")]
        public string PrivacyState { get; set; }
        [XmlElement(ElementName = "visibilityState")]
        public string VisibilityState { get; set; }
        [XmlElement(ElementName = "avatarIcon")]
        public string AvatarIcon { get; set; }
        [XmlElement(ElementName = "avatarMedium")]
        public string AvatarMedium { get; set; }
        [XmlElement(ElementName = "avatarFull")]
        public string AvatarFull { get; set; }
        [XmlElement(ElementName = "vacBanned")]
        public string VacBanned { get; set; }
        [XmlElement(ElementName = "tradeBanState")]
        public string TradeBanState { get; set; }
        [XmlElement(ElementName = "isLimitedAccount")]
        public string IsLimitedAccount { get; set; }
        [XmlElement(ElementName = "customURL")]
        public string CustomURL { get; set; }
        [XmlElement(ElementName = "memberSince")]
        public string MemberSince { get; set; }
        [XmlElement(ElementName = "steamRating")]
        public string SteamRating { get; set; }
        [XmlElement(ElementName = "hoursPlayed2Wk")]
        public double HoursPlayed2Wk { get; set; }
        [XmlElement(ElementName = "headline")]
        public string Headline { get; set; }
        [XmlElement(ElementName = "location")]
        public string Location { get; set; }
        [XmlElement(ElementName = "realname")]
        public string Realname { get; set; }
        [XmlElement(ElementName = "summary")]
        public string Summary { get; set; }
        [XmlElement(ElementName = "mostPlayedGames")]
        public MostPlayedGames MostPlayedGames { get; set; }
        [XmlElement(ElementName = "groups")]
        public Groups Groups { get; set; }
    }

    [XmlRoot(ElementName = "mostPlayedGame")]
    public class MostPlayedGame
    {
        [XmlElement(ElementName = "gameName")]
        public string GameName { get; set; }
        [XmlElement(ElementName = "gameLink")]
        public string GameLink { get; set; }
        [XmlElement(ElementName = "gameIcon")]
        public string GameIcon { get; set; }
        [XmlElement(ElementName = "gameLogo")]
        public string GameLogo { get; set; }
        [XmlElement(ElementName = "gameLogoSmall")]
        public string GameLogoSmall { get; set; }
        [XmlElement(ElementName = "hoursPlayed")]
        public string HoursPlayed { get; set; }
        [XmlElement(ElementName = "hoursOnRecord")]
        public string HoursOnRecord { get; set; }
        [XmlElement(ElementName = "statsName")]
        public string StatsName { get; set; }
    }

    [XmlRoot(ElementName = "mostPlayedGames")]
    public class MostPlayedGames
    {
        [XmlElement(ElementName = "mostPlayedGame")]
        public List<MostPlayedGame> MostPlayedGame { get; set; }
    }

    [XmlRoot(ElementName = "group")]
    public class Group
    {
        [XmlElement(ElementName = "groupID64")]
        public string GroupID64 { get; set; }
        [XmlElement(ElementName = "groupName")]
        public string GroupName { get; set; }
        [XmlElement(ElementName = "groupURL")]
        public string GroupURL { get; set; }
        [XmlElement(ElementName = "headline")]
        public string Headline { get; set; }
        [XmlElement(ElementName = "summary")]
        public string Summary { get; set; }
        [XmlElement(ElementName = "avatarIcon")]
        public string AvatarIcon { get; set; }
        [XmlElement(ElementName = "avatarMedium")]
        public string AvatarMedium { get; set; }
        [XmlElement(ElementName = "avatarFull")]
        public string AvatarFull { get; set; }
        [XmlElement(ElementName = "memberCount")]
        public string MemberCount { get; set; }
        [XmlElement(ElementName = "membersInChat")]
        public string MembersInChat { get; set; }
        [XmlElement(ElementName = "membersInGame")]
        public string MembersInGame { get; set; }
        [XmlElement(ElementName = "membersOnline")]
        public string MembersOnline { get; set; }
        [XmlAttribute(AttributeName = "isPrimary")]
        public string IsPrimary { get; set; }
    }

    [XmlRoot(ElementName = "groups")]
    public class Groups
    {
        [XmlElement(ElementName = "group")]
        public List<Group> Group { get; set; }
    }
}
