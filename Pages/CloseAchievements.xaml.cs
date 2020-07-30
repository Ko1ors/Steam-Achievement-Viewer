
using System.Windows.Controls;


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
