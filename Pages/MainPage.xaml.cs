using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace AchievementTest.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void ButtonEnter_Click(object sender, RoutedEventArgs e)
        {
            GetUserInformation();
            UpdateProgressBar(0);
        }

        private void GetUserInformation()
        {
            string steamID = textBoxSteamID.Text;
            Task.Run(new Action(() =>
            {
                UpdateStatusLabel("Получаю данные о профиле");
                Thread.Sleep(1000);

                if (Manager.GetProfile(steamID))
                {
                    UpdateStatusLabel("Данные о профиле были успешно получены");
                }
                else
                {
                    UpdateStatusLabel("Не удалось получить данные о профиле. Подождите и попробуйте позже");
                    return;
                }
                Thread.Sleep(1000);
                UpdateStatusLabel("Получаю список игр");
                Thread.Sleep(1000);

                if (Manager.GetGames())
                {
                    UpdateStatusLabel("Список игр был успешно получен");
                }
                else
                {
                    UpdateStatusLabel("Не удалось получить список игр. Подождите и попробуйте позже");
                    return;
                }

                Thread.Sleep(1000);
                UpdateStatusLabel("Получаю список достижений");

                Task.Run(new Action(() =>
                {
                    while(Manager.currentGameRetrieve < Manager.gamesList.Games.Game.Count)
                    {
                        UpdateStatusLabel("Получаю список достижений "
                            + Manager.currentGameRetrieve + "/" + Manager.gamesList.Games.Game.Count
                            + "\t" + Manager.gamesList.Games.Game[Manager.currentGameRetrieve].Name);
                        UpdateProgressBar((Manager.currentGameRetrieve * 100)/Manager.gamesList.Games.Game.Count);
                        Thread.Sleep(1000);
                    }
                }));

                if (Manager.GetAchievements())
                {
                    UpdateStatusLabel("Список достижений был успешно получен");
                }
                else
                {
                    UpdateStatusLabel("Не удалось получить список достижений. Подождите и попробуйте позже");
                    return;
                }
            }));
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
    }
}
