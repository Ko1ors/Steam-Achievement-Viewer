using SteamAchievementViewer.Services;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace SteamAchievementViewer.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        private INavigationService _navigationService;

        private bool _giip;
        private bool GetInformationInProcess
        {
            get => _giip;
            set
            {
                _giip = value;
                if (value)
                    EnableEnterButton(false);
                else
                    EnableEnterButton(true);
            }
        }
        public AuthPage(INavigationService navigationService)
        {
            InitializeComponent();
            _navigationService = navigationService;
        }

        private void ButtonEnter_Click(object sender, RoutedEventArgs e)
        {
            UpdateProgressBar(0);
            GetUserInformation();
        }

        private async Task GetUserInformation()
        {
            if (!GetInformationInProcess)
            {
                GetInformationInProcess = true;
                _navigationService.ChangeAvailability(false);
                string steamID = textBoxSteamID.Text;
                var task = Task.Run(new  Action(async () =>
                {
                    UpdateStatusLabel(Properties.Resources.RetrievingProfileData);
                    Thread.Sleep(1000);

                    if (await Manager.GetProfile(steamID))
                    {
                        Dispatcher.BeginInvoke(DispatcherPriority.Background, (SendOrPostCallback)delegate
                        {
                            (App.Current.MainWindow as MainWindow).UpdateAvatar(Manager.profile.AvatarFull);
                        }, null);

                        UpdateStatusLabel(Properties.Resources.ProfileDataRetrieved);
                    }
                    else
                    {
                        UpdateStatusLabel(Properties.Resources.ProfileDataFailed);
                        _navigationService.ChangeAvailability(true);
                        return;
                    }
                    Thread.Sleep(1000);
                    UpdateStatusLabel(Properties.Resources.RetrievingGameList);
                    Thread.Sleep(1000);

                    if (await Manager.GetGames())
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

                    Task.Run(new Action(() =>
                    {
                        while (Manager.currentGameRetrieve < Manager.gamesList.Games.Game.Count)
                        {
                            UpdateStatusLabel(Properties.Resources.RetrievingAchievementList + " " +
                                + Manager.currentGameRetrieve + "/" + Manager.gamesList.Games.Game.Count
                                + "\t" + Manager.gamesList.Games.Game[Manager.currentGameRetrieve].Name);
                            UpdateProgressBar((Manager.currentGameRetrieve * 100) / Manager.gamesList.Games.Game.Count);
                            Thread.Sleep(1000);
                        }
                        UpdateProgressBar((Manager.currentGameRetrieve * 100) / Manager.gamesList.Games.Game.Count);
                    }));

                    if (await Manager.GetAchievementsParallel())
                    {
                        UpdateStatusLabel(Properties.Resources.AchievementListRetrieved);
                    }
                    else
                    {
                        UpdateStatusLabel(Properties.Resources.AchievementListFailed);
                        _navigationService.ChangeAvailability(true);
                        return;
                    }
                    Thread.Sleep(1000);
                    UpdateStatusLabel(Properties.Resources.ResultSaving);
                    Manager.SaveProfile();
                    Manager.SaveGames();
                    Manager.SaveSettingsInfo();
                    Thread.Sleep(1000);
                    UpdateStatusLabel(Properties.Resources.ResultSaved);
                    _navigationService.ChangeAvailability(true);

                }));
                Task.Run(() =>
                {
                    task.Wait();
                    GetInformationInProcess = false;
                });
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
