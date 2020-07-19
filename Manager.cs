using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace AchievementTest
{
    public static class Manager
    {
        static Profile profile;
        private static readonly string xmlProfileError = "The specified profile could not be found.";
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
            return true;
        }
    }
}
