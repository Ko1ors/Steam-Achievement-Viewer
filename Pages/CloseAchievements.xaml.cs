
using SteamAchievementViewer.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;

namespace SteamAchievementViewer.Pages
{
    /// <summary>
    /// Логика взаимодействия для CloseAchievements.xaml
    /// </summary>
    public partial class CloseAchievements : Page
    {
        public CloseAchievements()
        {
            InitializeComponent();
        }


        private void GameSelected(object sender, SelectionChangedEventArgs e)
        {
            if (GameList.SelectedItem != null)
            {
                AchievementList.ItemsSource = Manager.GetClosestAchievements((GameList.SelectedItem as Game).AppID).Achievement;
                if (AchievementList.Items.Count > 0)
                    AchievementList.ScrollIntoView(AchievementList.Items[0]);
            }
            else
                AchievementList.ItemsSource = null;
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
            CollectionViewSource.GetDefaultView(GameList.ItemsSource)?.Refresh();
        }

        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (Manager.IsLogged())
            {
                GameList.ItemsSource = Manager.GetIncompleteGames();
                ListCollectionView view = (ListCollectionView)CollectionViewSource.GetDefaultView(GameList.ItemsSource);
                view.Filter = GameSearchFilter;
            }
        }
    }
}
