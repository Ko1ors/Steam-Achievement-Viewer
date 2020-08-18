using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace SteamAchievementViewer
{
    class GetRequest
    {
        public static XmlDocument XmlRequest(string RequestUrl)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(RequestUrl);
            request.Method = "GET";
            request.ContentType = "text/xml";
            XmlDocument doc = new XmlDocument();
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    doc.Load(new StreamReader(response.GetResponseStream(), Encoding.UTF8));
                }
            }
            catch
            {

            }
            return doc;
        }
    }
}
