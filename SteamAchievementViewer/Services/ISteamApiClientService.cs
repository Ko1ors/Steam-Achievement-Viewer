using SteamAchievementViewer.Models.SteamApi;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SteamAchievementViewer.Services
{
    public interface ISteamApiClientService : IClientService<string>
    {
        public Task<IEnumerable<Game>> GetOwnedGamesAsync(string steamId, string steamApiKey);
    }
}
