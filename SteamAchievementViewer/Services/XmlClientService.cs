using Sav.Common.Logs;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SteamAchievementViewer.Services
{
    public class XmlClientService : IClientService<XmlDocument>
    {
        public async Task<XmlDocument> SendGetRequest(string requestUrl)
        {
            Log.Logger.Information("Requesting {RequestUrl}", requestUrl);

            XmlDocument doc = new XmlDocument();
            try
            {
                using HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("Accept", "text/xml;charset=UTF-8");
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(requestUrl),
                };
                var response = await client.SendAsync(request).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                doc.Load(new StreamReader(await response.Content.ReadAsStreamAsync(), Encoding.UTF8));
                Log.Logger.Information("Request successful, {RequestUrl}", requestUrl);
                return doc;
            }
            catch (Exception e)
            {
                Log.Logger.Error(e, "Failed to request {RequestUrl}", requestUrl);
                return doc;
            }
        }
    }
}
