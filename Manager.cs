using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace AchievementTest
{
    public static class Manager
    {
        public static Profile profile;
        public static GamesList gamesList;
        private static readonly string xmlProfileError = "The specified profile could not be found.";
        private static readonly string directoryPath = AppDomain.CurrentDomain.BaseDirectory;

        public static void Start()
        {

        }

        public static bool GetProfile(string steamID)
        {
            var response = GetRequest.XmlRequest("https://steamcommunity.com/profiles/" + steamID + "/?xml=1");
            if (response.InnerText == xmlProfileError)
                return false;
            XmlSerializer serializer = new XmlSerializer(typeof(Profile));
            using (XmlReader reader = new XmlNodeReader(response))
            {
                profile = (Profile)serializer.Deserialize(reader);
            }
            if (profile.PrivacyState != "public")
            {
                profile = null;
                return false;
            }
            SaveProfile();
            return true;
        }

        public static void SaveProfile()
        {
            if(!System.IO.Directory.Exists(directoryPath + profile.SteamID64))
                Directory.CreateDirectory(directoryPath + profile.SteamID64);
            XmlSerializer serializer = new XmlSerializer(typeof(Profile));
            System.IO.FileStream file = System.IO.File.Create(directoryPath + profile.SteamID64 + "\\profile.xml");
            serializer.Serialize(file,profile);
        }

        public static void LoadProfile()
        {
            if (File.Exists(directoryPath + profile.SteamID64 + "\\profile.xml"))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Profile));
                System.IO.StreamReader file = new System.IO.StreamReader(directoryPath + profile.SteamID + "\\profile.xml");
                profile = (Profile)serializer.Deserialize(file);
                file.Close();
            }
        }

        public static bool GetGames()
        {
            var response = GetRequest.XmlRequest("https://steamcommunity.com/profiles/" + profile.SteamID64 + "/games?tab=all&xml=1");
            if (response.InnerText == xmlProfileError)
                return false;
            XmlSerializer serializer = new XmlSerializer(typeof(GamesList));
            using (XmlReader reader = new XmlNodeReader(response))
            {
                gamesList = (GamesList)serializer.Deserialize(reader);
            }
            if(gamesList.Games.Game.Count == 0)
            {
                gamesList = null;
                return false;
            }
            return true;
        }
    }
}
