using SteamAchievementViewer.Models;
using SteamAchievementViewer.Services;
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
        private readonly INavigationService _navigationService;

        public MainWindowModel Model { get; set; }

        public MainWindow(INavigationService navigationService)
        {
            InitializeComponent();
            _navigationService = navigationService;
            Model = new MainWindowModel();
            Model.NavigationPages = _navigationService.GetPageElements().ToList();
            Model.IsNavigationAvailable = true;
            _navigationService.SetNavigationFrame(Information);
            _navigationService.NavigateTo(Model.NavigationPages.First());

            DataContext = this;

            Manager.Start();
            if (Manager.profile != null)
                UpdateAvatar(Manager.profile.AvatarFull);
        }

        public void UpdateAvatar(string IconPath)
        {
            ProfileAvatar.Source = new BitmapImage(new Uri(IconPath));
        }

        private void Navigation_Click(object sender, RoutedEventArgs e)
        {
            if (Model.IsNavigationAvailable)
            {
                var button = (Button)sender;
                var navigationPage = button.CommandParameter as NavigationPageElement;
                _navigationService.NavigateTo(navigationPage);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
