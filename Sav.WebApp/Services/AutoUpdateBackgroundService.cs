using Sav.Common.Interfaces;
using Sav.Common.Services;

namespace Sav.WebApp.Services
{
    /// <summary>
    /// Refreshes the saved profile and queue on the configured interval (v1 did the same check on every launch).
    /// </summary>
    public class AutoUpdateBackgroundService : BackgroundService
    {
        private static readonly TimeSpan CheckInterval = TimeSpan.FromMinutes(1);

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<AutoUpdateBackgroundService> _logger;

        public AutoUpdateBackgroundService(IServiceScopeFactory scopeFactory, ILogger<AutoUpdateBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(CheckInterval);

            do
            {
                await TryRunUpdateAsync(stoppingToken);
            }
            while (await timer.WaitForNextTickAsync(stoppingToken));
        }

        private async Task TryRunUpdateAsync(CancellationToken stoppingToken)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var settingsService = scope.ServiceProvider.GetRequiredService<ISettingsService>();
                var settings = settingsService.Get();

                if (string.IsNullOrWhiteSpace(settings.SteamID))
                    return;

                var interval = TimeSpan.FromMinutes(Math.Max(1, settings.AutoUpdateIntervalMinutes));
                if (settings.LastAutoUpdateUtc is { } lastUpdate && DateTime.UtcNow - lastUpdate < interval)
                    return;

                var steamId = settings.SteamID;
                _logger.LogInformation("Auto update started for {SteamId}, interval {Interval}", steamId, interval);

                var steamService = scope.ServiceProvider.GetRequiredService<ISteamService>();
                var profileResult = await steamService.TryUpdateProfileAsync(steamId);

                if (profileResult == ProfileUpdateResult.Success)
                {
                    var gamesResult = await steamService.TryUpdateGamesAsync(steamId);
                    _logger.LogInformation("Auto update game list result for {SteamId}: {Result}", steamId, gamesResult);
                }

                steamService.QueueAchievementsUpdate();

                settings.LastAutoUpdateUtc = DateTime.UtcNow;
                settingsService.Save(settings);

                _logger.LogInformation("Auto update finished for {SteamId}, profile {ProfileResult}", steamId, profileResult);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // Normal shutdown.
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Auto update failed");
            }
        }
    }
}
