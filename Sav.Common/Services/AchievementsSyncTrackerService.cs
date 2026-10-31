using Sav.Common.Interfaces;
using Sav.Common.Models;
using Sav.Infrastructure.Entities;

namespace Sav.Common.Services
{
    public class AchievementsSyncTrackerService : IAchievementsSyncTrackerService
    {
        private readonly object _lock = new();
        private readonly IQueueService<UserGameEntity> _queueService;

        private int _activeWorkers;
        private int _processedGames;
        private int _totalGames;
        private bool _isRunning;
        private string? _lastGameName;
        private DateTime? _startedAt;
        private DateTime? _finishedAt;

        public AchievementsSyncTrackerService(IQueueService<UserGameEntity> queueService)
        {
            _queueService = queueService;
        }

        public AchievementsSyncProgress GetProgress()
        {
            lock (_lock)
            {
                return new AchievementsSyncProgress
                {
                    IsRunning = _isRunning || _activeWorkers > 0 || _queueService.Size > 0,
                    TotalGames = _totalGames,
                    ProcessedGames = _processedGames,
                    QueueSize = _queueService.Size,
                    LastGameName = _lastGameName,
                    StartedAt = _startedAt,
                    FinishedAt = _finishedAt
                };
            }
        }

        public void OnGameProcessingStarted()
        {
            lock (_lock)
            {
                if (!_isRunning)
                {
                    _isRunning = true;
                    _processedGames = 0;
                    _totalGames = _queueService.Size + 1;
                    _lastGameName = null;
                    _startedAt = DateTime.Now;
                    _finishedAt = null;
                }

                _activeWorkers++;
                _totalGames = Math.Max(_totalGames, _processedGames + _activeWorkers + _queueService.Size);
            }
        }

        public void OnGameProcessed(string? gameName)
        {
            lock (_lock)
            {
                _processedGames++;
                if (!string.IsNullOrWhiteSpace(gameName))
                    _lastGameName = gameName;

                CompleteGame();
            }
        }

        public void OnGameProcessingFailed()
        {
            lock (_lock)
            {
                CompleteGame();
            }
        }

        private void CompleteGame()
        {
            _activeWorkers = Math.Max(0, _activeWorkers - 1);
            _totalGames = Math.Max(_totalGames, _processedGames + _activeWorkers + _queueService.Size);

            if (_activeWorkers == 0 && _queueService.Size == 0)
            {
                _isRunning = false;
                _finishedAt = DateTime.Now;
            }
        }
    }
}
