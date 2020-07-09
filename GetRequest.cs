using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace AchievementTest
{
    class GetRequest
    {
        public static XmlDocument XmlRequest(string RequestUrl) {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(RequestUrl);
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(new StreamReader(response.GetResponseStream(), Encoding.UTF8));
                return doc;
            }
        }
    }
}
