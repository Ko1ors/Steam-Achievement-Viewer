using SteamAchievementViewer.Services;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace SteamAchievementViewer.Pages
{
    public partial class AuthPage : Page
    {
        private INavigationService _navigationService;
        private ISteamService _steamService;

        private bool _getInformationInProcess;
        private bool GetInformationInProcess
        {
            get => _getInformationInProcess;
            set
            {
                _getInformationInProcess = value;
                if (value)
                    EnableEnterButton(false);
                else
                    EnableEnterButton(true);
            }
        }
        
        public AuthPage(INavigationService navigationService, ISteamService steamService)
        {
            InitializeComponent();
            _navigationService = navigationService;
            _steamService = steamService;
            _steamService.OnAchievementProgressUpdated += _steamService_OnAchievementProgressUpdated;
        }

        private void _steamService_OnAchievementProgressUpdated(int totalGames, int currentGameCount, string lastGameName)
        {

            UpdateStatusLabel(Properties.Resources.RetrievingAchievementList + " " + currentGameCount + "/" + totalGames + "\t" + lastGameName);
            UpdateProgressBar((currentGameCount * 100) / totalGames);
        }

        private void ButtonEnter_Click(object sender, RoutedEventArgs e)
        {
            _ = GetUserInformationAsync();
        }

        private async Task GetUserInformationAsync()
        {
            if (!GetInformationInProcess)
            {
                UpdateProgressBar(0);
                GetInformationInProcess = true;
                _navigationService.ChangeAvailability(false);
                string steamID = textBoxSteamID.Text;
                await Task.Run(async () =>
                {
                    UpdateStatusLabel(Properties.Resources.RetrievingProfileData);
                    Thread.Sleep(1000);

                    if (await _steamService.GetProfileAsync(steamID))
                    {
                        _ = Dispatcher.BeginInvoke(DispatcherPriority.Background, (SendOrPostCallback)delegate
                        {
                            (Application.Current.MainWindow as MainWindow).UpdateAvatar(_steamService.Profile.AvatarFull);
                        }, null);

                        UpdateStatusLabel(Properties.Resources.ProfileDataRetrieved);
                    }
                    else
                    {
                        UpdateStatusLabel(Properties.Resources.ProfileDataFailed);
                        return;
                    }

                    Thread.Sleep(1000);
                    UpdateStatusLabel(Properties.Resources.RetrievingGameList);
                    Thread.Sleep(1000);

                    if (await _steamService.GetGamesAsync(steamID))
                    {
                        UpdateStatusLabel(Properties.Resources.GameListRetrieved);
                    }
                    else
                    {
                        UpdateStatusLabel(Properties.Resources.GameListFailed);
                        _navigationService.ChangeAvailability(true);
                        return;
                    }

                    Thread.Sleep(1000);
                    UpdateStatusLabel(Properties.Resources.RetrievingAchievementList);

                    if (await _steamService.GetAchievementsParallelAsync(_steamService.GamesList.Games.Game))
                    {
                        UpdateStatusLabel(Properties.Resources.AchievementListRetrieved);
                    }
                    else
                    {
                        UpdateStatusLabel(Properties.Resources.AchievementListFailed);
                        return;
                    }

                    Thread.Sleep(1000);
                    UpdateStatusLabel(Properties.Resources.ResultSaving);

                    _steamService.SaveProfile();
                    _steamService.SaveGames();
                    _steamService.SaveSettingsInfo();

                    Thread.Sleep(1000);
                    UpdateStatusLabel(Properties.Resources.ResultSaved);
                });

                _navigationService.ChangeAvailability(true);
                GetInformationInProcess = false;
            }
        }

        private void UpdateProgressBar(int Progress)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, (SendOrPostCallback)delegate
            {
                CircleProgressBar.Value = Progress;
            }, null);
        }
        private void UpdateStatusLabel(string message)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, (SendOrPostCallback)delegate
            {
                statusLabel.Content = message;
            }, null);
        }

        private void EnableEnterButton(bool isenabled)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, (SendOrPostCallback)delegate
            {
                EnterButton.IsEnabled = isenabled;
            }, null);
        }
    }
}