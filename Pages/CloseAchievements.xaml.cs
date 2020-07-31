
using System;
using System.Windows.Controls;
using System.Windows.Data;

namespace AchievementTest.Pages
{
    /// <summary>
    /// Логика взаимодействия для CloseAchievements.xaml
    /// </summary>
    public partial class CloseAchievements : Page
    {
        public CloseAchievements()
        {
            InitializeComponent();
            GameList.ItemsSource = Manager.GetIncompleteGames();
        }


        private void GameSelected(object sender, SelectionChangedEventArgs e)
        {
            
            AchievementList.ItemsSource = Manager.GetClosestAchievements((GameList.SelectedItem as Game).AppID).Achievement;
            if (AchievementList.Items.Count > 0)
                AchievementList.ScrollIntoView(AchievementList.Items[0]);
        }
    }
}
