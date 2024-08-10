using Sav.Common.Interfaces;
using Sav.Common.Logs;
using Sav.Infrastructure.Entities;
using SteamAchievementViewer.Commands;
using SteamAchievementViewer.Services;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SteamAchievementViewer.ViewModels
{
    public class AuthPageViewModel : ViewModelBase
    {
        private const string HyperlinkUrl = "https://github.com/Ko1ors/Steam-Achievement-Viewer/blob/master/README.md#login";
        private readonly TimeSpan ProfileUpdateInterval = TimeSpan.FromHours(1);

        private readonly INavigationService _navigationService;
        private readonly ISteamService _steamService;
        private readonly IQueueService<UserGameEntity> _queueService;

        private CancellationTokenSource _cancellationTokenSource;

        private bool _getInformationInProcess;
        private string _statusLabelContent;
        private int _progressBarValue;
        private string _steamId;
        private int _gamesInQueue;

        public string StatusLabelContent
        {
            get { return _statusLabelContent; }
            set
            {
                _statusLabelContent = value;
                OnPropertyChanged(nameof(StatusLabelContent));
            }
        }

        public int ProgressBarValue
        {
            get { return _progressBarValue; }
            set
            {
                _progressBarValue = value;
                OnPropertyChanged(nameof(ProgressBarValue));
            }
        }

        public string SteamId
        {
            get { return _steamId; }
            set
            {
                _steamId = value;
                OnPropertyChanged(nameof(SteamId));
            }
        }

        public int GamesInQueue
        {
            get { return _gamesInQueue; }
            set
            {
                _gamesInQueue = value;
                OnPropertyChanged(nameof(GamesInQueue));
                OnPropertyChanged(nameof(HasGamesInQueue));
            }
        }

        public bool HasGamesInQueue => GamesInQueue > 0;

        public RelayCommand AuthCommand { get; set; }

        public RelayCommand HyperlinkCommand { get; set; }

        public AuthPageViewModel(INavigationService navigationService, ISteamService steamService, IQueueService<UserGameEntity> queueService)
        {
            _navigationService = navigationService;
            _steamService = steamService;
            _queueService = queueService;
            ProfileUpdateInterval = Settings.Default.UpdateInterval;
            _cancellationTokenSource = new CancellationTokenSource();

            UpdateNavigationAvailability();
            UpdateSteamData();
            Task.Run(() => RunQueuePolling());

            AuthCommand = new RelayCommand((obj) => _ = GetUserInformationAsync(Settings.Default.UpdateOnlyRecentGames), (obj) => CanAuth());
            HyperlinkCommand = new RelayCommand((obj) => Process.Start(new ProcessStartInfo("cmd", $"/c start {HyperlinkUrl}") { CreateNoWindow = true }));
        }

        private void UpdateNavigationAvailability()
        {
            if (!_steamService.IsLogged())
            {
                _navigationService.ChangeAvailability(false);
                return;
            }

            var user = _steamService.GetUser();
            if (user is null)
            {
                _navigationService.ChangeAvailability(false);
                return;
            }

            _navigationService.ChangeAvailability(true);
        }

        private void UpdateSteamData()
        {
            Log.Logger.Information("Updating Steam data");

            if (!_steamService.IsLogged())
                return;

            SteamId = _steamService.GetUserId();

            var user = _steamService.GetUser();
            if (user is null)
                return;

            if (user.Updated + ProfileUpdateInterval <= DateTime.Now)
                _ = GetUserInformationAsync(Settings.Default.UpdateOnlyRecentGames);
            else
                _steamService.QueueAchievementsUpdate();
        }

        private bool CanAuth()
        {
            return !_getInformationInProcess && !string.IsNullOrWhiteSpace(SteamId);
        }

        private async Task RunQueuePolling()
        {
            var timer = new PeriodicTimer(TimeSpan.FromSeconds(5));
            while (await timer.WaitForNextTickAsync(_cancellationTokenSource.Token))
            {
                GamesInQueue = _queueService.Size;
            }
        }

        private async Task GetUserInformationAsync(bool queueOnlyRecentGames = false)
        {
            try
            {
                await Task.Run(async () =>
                {
                    Log.Logger.Information("Getting user information {SteamId}", SteamId);

                    _getInformationInProcess = true;
                    ProgressBarValue = 0;
                    _navigationService.ChangeAvailability(false);
                    string steamID = SteamId;

                    StatusLabelContent = Properties.Resources.RetrievingProfileData;
                    await Task.Delay(1000);

                    if (await _steamService.UpdateProfileAsync(steamID))
                    {
                        StatusLabelContent = Properties.Resources.ProfileDataRetrieved;
                        Log.Logger.Information("Profile data retrieved {SteamId}", SteamId);
                    }
                    else
                    {
                        StatusLabelContent = Properties.Resources.ProfileDataFailed;
                        Log.Logger.Error("Profile data failed {SteamId}", SteamId);
                        return;
                    }

                    await Task.Delay(1000);
                    StatusLabelContent = Properties.Resources.RetrievingGameList;
                    Log.Logger.Information("Retrieving game list {SteamId}", SteamId);
                    await Task.Delay(1000);

                    if (await _steamService.UpdateGamesAsync(steamID))
                    {
                        StatusLabelContent = Properties.Resources.GameListRetrieved;
                        Log.Logger.Information("Game list retrieved {SteamId}", SteamId);
                    }
                    else
                    {
                        StatusLabelContent = Properties.Resources.GameListFailed;
                        Log.Logger.Error("Game list failed {SteamId}", SteamId);
                        return;
                    }

                    await Task.Delay(1000);
                    StatusLabelContent = Properties.Resources.RetrievingAchievementList;
                    Log.Logger.Information("Retrieving achievement list {SteamId}", SteamId);

                    await Task.Delay(1000);
                    StatusLabelContent = Properties.Resources.ResultSaving;
                    Log.Logger.Information("Saving settings {SteamId}", SteamId);

                    _steamService.SaveSettingsInfo();

                    _steamService.QueueAchievementsUpdate(queueOnlyRecentGames);

                    await Task.Delay(1000);
                    StatusLabelContent = Properties.Resources.ResultSaved;
                }).ContinueWith((t) =>
                {
                    if (t.IsFaulted) 
                    { 
                        Log.Logger.Error(t.Exception, "Error while getting user information {SteamId}", SteamId);
                        throw t.Exception;
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Error while getting user information {SteamId}", SteamId);
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                _getInformationInProcess = false;
                Log.Logger.Information("Getting user information finished {SteamId}", SteamId);
                UpdateNavigationAvailability();
            }
        }

        private void SteamServiceOnAchievementProgressUpdated(int totalGames, int currentGameCount, string lastGameName)
        {
            StatusLabelContent = Properties.Resources.RetrievingAchievementList + " " + currentGameCount + "/" + totalGames + "\t" + lastGameName;
            ProgressBarValue = currentGameCount * 100 / totalGames;
        }
    }
}
