using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace SteamAchievementViewer.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        private string hyperlinkText;
        public string HyperlinkText
        {
            get => hyperlinkText;
            set
            {
                hyperlinkText = value;
            }
        }

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
        public AuthPage()
        {
            InitializeComponent();
            HyperlinkText = "https://github.com/Ko1ors/Steam-Achievement-Viewer/blob/master/README.md#login";
        }
        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("cmd", $"/c start {HyperlinkText}") { CreateNoWindow = true });
        }
        private void ButtonEnter_Click(object sender, RoutedEventArgs e)
        {
            UpdateProgressBar(0);
            GetUserInformation();
        }

        private void GetUserInformation()
        {
            if (!GetInformationInProcess)
            {
                GetInformationInProcess = true;
                string steamID = textBoxSteamID.Text;
                var task = Task.Run(new Action(() =>
                {
                    UpdateStatusLabel(Properties.Resources.RetrievingProfileData);
                    Thread.Sleep(1000);

                    if (Manager.GetProfile(steamID))
                    {
                        Dispatcher.BeginInvoke(DispatcherPriority.Background, (SendOrPostCallback)delegate
                        {
                            (System.Windows.Application.Current.MainWindow as MainWindow).UpdateAvatar(Manager.profile.AvatarFull);
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

                    if (Manager.GetGames())
                    {
                        UpdateStatusLabel(Properties.Resources.GameListRetrieved);
                    }
                    else
                    {
                        UpdateStatusLabel(Properties.Resources.GameListFailed);
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

                    if (Manager.GetAchievementsParallel())
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
                    Manager.SaveProfile();
                    Manager.SaveGames();
                    Manager.SaveSettingsInfo();
                    Thread.Sleep(1000);
                    UpdateStatusLabel(Properties.Resources.ResultSaved);
                    
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
