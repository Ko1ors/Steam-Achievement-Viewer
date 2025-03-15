using Sav.Common.Models.SteamApi;

namespace Sav.Common.Services
{
    public interface ISteamApiClientService : IClientService<string>
    {
        public Task<IEnumerable<Game>> GetOwnedGamesAsync(string steamId, string steamApiKey);
    }
}
