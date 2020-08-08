using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AchievementTest.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainPageInfo.xaml
    /// </summary>
    public partial class MainPageInfo : Page
    {
        public MainPageInfo()
        {
            InitializeComponent();
        }
        public void UpdateProfileName(string Name)
        {
            ProfileName.Text = Name;
        }
        public void UpdateGameCount(int GameCount)
        {
            GamesCount.Text = GameCount.ToString();
        }
        public void  UpdateAchievementCount(int Achievements)
        {
            AchievementCount.Text = Achievements.ToString();
        }
        public void UpdateAchieved(int Achieved)
        {
            AchievementAchieved.Text = Achieved.ToString();
            AchievementAchieved.Text += "/";
            AchievementAchieved.Text += AchievementCount.Text;
        }
        public void UpdateAchievementStatistics(int Achieved)
        {
            AchievementStatistics.Text = Achieved.ToString();
            AchievementStatistics.Text += "/100%";
        }
        private void UpdateProgressBar(int Progress)
        {
            AchievementProgressCircle.Value = Progress;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (Manager.IsLogged())
            {
                infoStackPanel.Visibility = Visibility.Visible;
                UpdateProfileName(Manager.profile.SteamID);
                UpdateGameCount(Manager.gamesList.Games.Game.Count);
                UpdateAchievementCount(Manager.GetTotalAchievementsCount());
                UpdateAchieved(Manager.GetCompletedAchievementsCount());
                UpdateAchievementStatistics((Manager.GetCompletedAchievementsCount() * 100) / Manager.GetTotalAchievementsCount());
                UpdateProgressBar((Manager.GetCompletedAchievementsCount() * 100) / Manager.GetTotalAchievementsCount());
            }
            else
                infoStackPanel.Visibility = Visibility.Collapsed;
        }
    }
}
