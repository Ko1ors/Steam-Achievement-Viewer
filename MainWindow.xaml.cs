using SteamAchievementViewer.Services;
using SteamAchievementViewer.ViewModels;
using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace SteamAchievementViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow(MainWindowViewModel viewModel, ISteamService steamService)
        {
            InitializeComponent();
            DataContext = viewModel;

            // TODO: move update avatar logic into view model
            if (steamService.Profile != null)
                UpdateAvatar(steamService.Profile.AvatarFull);
        }

        public void UpdateAvatar(string IconPath)
        {
            ProfileAvatar.Source = new BitmapImage(new Uri(IconPath));
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
