using SteamAchievementViewer.Models;
using SteamAchievementViewer.Services;
using SteamAchievementViewer.ViewModels;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace SteamAchievementViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            Manager.Start();
            if (Manager.profile != null)
                UpdateAvatar(Manager.profile.AvatarFull);
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
