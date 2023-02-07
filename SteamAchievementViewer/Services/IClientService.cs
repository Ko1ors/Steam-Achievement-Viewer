using System.Threading.Tasks;

namespace SteamAchievementViewer.Services
{
    public interface IClientService<T> where T : class
    {
        Task<T> SendGetRequest(string requestUrl);
    }
}
