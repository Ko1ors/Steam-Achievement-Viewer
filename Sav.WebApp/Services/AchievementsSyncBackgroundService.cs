using Sav.Common.Interfaces;

namespace Sav.WebApp.Services
{
    public class AchievementsSyncBackgroundService : BackgroundService
    {
        private const int DefaultWorkerCount = 5;

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AchievementsSyncBackgroundService> _logger;

        public AchievementsSyncBackgroundService(IServiceScopeFactory scopeFactory, IConfiguration configuration,
            ILogger<AchievementsSyncBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _configuration = configuration;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var workerCount = Math.Max(1, _configuration.GetValue("AchievementsSync:WorkerCount", DefaultWorkerCount));
            _logger.LogInformation("Starting {WorkerCount} achievements sync workers", workerCount);

            var workers = Enumerable.Range(0, workerCount).Select(index => RunWorkerAsync(index, stoppingToken));
            await Task.WhenAll(workers);

            _logger.LogInformation("Achievements sync workers stopped");
        }

        private async Task RunWorkerAsync(int index, CancellationToken stoppingToken)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var worker = scope.ServiceProvider.GetRequiredService<IAchievementsWorkerService>();
                await worker.StartAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // Normal shutdown.
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Achievements sync worker {Index} crashed", index);
            }
        }
    }
}
