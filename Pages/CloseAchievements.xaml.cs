
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
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(GameList.ItemsSource);
            view.Filter = GameSearchFilter;
        }


        private void GameSelected(object sender, SelectionChangedEventArgs e)
        {
            
            AchievementList.ItemsSource = Manager.GetClosestAchievements((GameList.SelectedItem as Game).AppID).Achievement;
            if (AchievementList.Items.Count > 0)
                AchievementList.ScrollIntoView(AchievementList.Items[0]);
        }

        private bool GameSearchFilter(object item)
        {
            if (String.IsNullOrEmpty(textBoxSearch.Text))
                return true;
            else
                return ((item as Game).Name.IndexOf(textBoxSearch.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private void textBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(GameList.ItemsSource).Refresh();
        }
    }
}
