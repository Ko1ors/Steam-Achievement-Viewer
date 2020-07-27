using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Логика взаимодействия для CloseAchievements.xaml
    /// </summary>
    public partial class CloseAchievements : Page
    {
        public CloseAchievements()
        {
            InitializeComponent();
            GameList.ItemsSource = Manager.gamesList.Games.Game;
        }

        private void GameSelected(object sender, SelectionChangedEventArgs e)
        {
            AchievementList.ItemsSource = Manager.GetClosestAchievements(Manager.gamesList.Games.Game[GameList.SelectedIndex].AppID).Achievement;
        }
    }
}
