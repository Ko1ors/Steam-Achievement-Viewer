using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Serialization;

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
        }

        private void GetUserInformation()
        {
            string steamID = textBoxSteamID.Text;
            UpdateStatusLabel("Получаю данные о профиле");
            Task.Run(new Action(() =>
            {
                Thread.Sleep(1000);
                if (Manager.GetProfile(steamID))
                {
                    UpdateStatusLabel("Данные о профиле были успешно получены");
                }
                else
                {
                    UpdateStatusLabel("Не удалось получить данные о профиле. Подождите и попробуйте позже");
                }
                Thread.Sleep(1000);
                UpdateStatusLabel("Не удалось получить данн");
            }));
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
