using Sav.Common.Interfaces;
using Sav.Common.Logs;
using System;

namespace Sav.Common.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly ISettingsService _settingsService;

        public CurrentUserService(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public string? SteamId
        {
            get
            {
                var steamId = _settingsService.Get().SteamID;
                return string.IsNullOrWhiteSpace(steamId) ? null : steamId;
            }
        }

        public bool IsAuthenticated => SteamId is not null;

        public void SetCurrentUser(string steamId)
        {
            if (string.IsNullOrWhiteSpace(steamId))
                throw new ArgumentException("Steam ID cannot be empty.", nameof(steamId));

            var settings = _settingsService.Get();
            settings.SteamID = steamId.Trim();
            _settingsService.Save(settings);

            Log.Logger.Information("Current user set to {SteamID}", settings.SteamID);
        }
    }
}
