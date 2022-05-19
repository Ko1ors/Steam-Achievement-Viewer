using SteamAchievementViewer.Models;
using SteamAchievementViewer.Pages;
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
        public MainWindowModel Model { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Model = new MainWindowModel();
            Model.NavigationPages = new()
            {
                new NavigationPageElement{ Type = typeof(MainPageInfo), Title = Properties.Resources.MainPage },
                new NavigationPageElement{ Type = typeof(AuthPage), Title = Properties.Resources.AuthPage },
                new NavigationPageElement{ Type = typeof(LastAchievedPage), Title = Properties.Resources.LastAchievedPage },
                new NavigationPageElement{ Type = typeof(CloseAchievements), Title = Properties.Resources.ClosestPage },
                new NavigationPageElement{ Type = typeof(CloseAllAchievements), Title = Properties.Resources.ClosestAllPage },
                new NavigationPageElement{ Type = typeof(RareAchievements), Title = Properties.Resources.RarestPage },
            };
            Model.IsNavigationAvailable = true;
            NavigateTo(Model.NavigationPages.First());

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
                NavigateTo(navigationPage);
            }
        }

        private void NavigateTo(NavigationPageElement navigationPage)
        {
            Model.NavigationPages.ForEach(np => np.Selected = false);
            navigationPage.Selected = true;
            Information.Content = Manager.GetPageObject(navigationPage.Type);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
