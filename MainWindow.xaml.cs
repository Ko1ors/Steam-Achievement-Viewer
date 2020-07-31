using AchievementTest.Pages;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace AchievementTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            Manager.Start();
            if (Manager.profile != null)
                UpdateAvatar(Manager.profile.AvatarFull);
        }

        public void UpdateAvatar(string IconPath)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, (SendOrPostCallback)delegate
            {
                ((MainWindow)System.Windows.Application.Current.MainWindow).ProfileAvatar.Source = new BitmapImage(new Uri(IconPath));
            }, null);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AuthorizationButton_clicked(object sender, RoutedEventArgs e)
        {
            Information.Content = Manager.GetPageObject<MainPage>();
        }
        private void LastAchievedButton_clicked(object sender, RoutedEventArgs e)
        {
            Information.Content = Manager.GetPageObject<LastAchievedPage>();
        }

        private void CloseAchievementButton_clicked(object sender, RoutedEventArgs e)
        {

            Information.Content = Manager.GetPageObject<CloseAchievements>();
        }

        private void RareAchievementButton_clicked(object sender, RoutedEventArgs e)
        {
            Information.Content = Manager.GetPageObject<RareAchievements>();
        }

        private void MainInfoButton_cliked(object sender, RoutedEventArgs e)
        {
            Information.Content = Manager.GetPageObject<MainPageInfo>();
        }
    }
}
