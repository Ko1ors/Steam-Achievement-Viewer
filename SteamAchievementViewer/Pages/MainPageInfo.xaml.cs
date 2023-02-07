using SteamAchievementViewer.Services;
using System.Windows;
using System.Windows.Controls;

namespace SteamAchievementViewer.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainPageInfo.xaml
    /// </summary>
    public partial class MainPageInfo : Page
    {
        private readonly ISteamService _steamService;
        private readonly IGameAchievementsService _gameAchievementsService;

        public MainPageInfo(ISteamService steamService, IGameAchievementsService gameAchievementsService)
        {
            InitializeComponent();
            _steamService = steamService;
            _gameAchievementsService = gameAchievementsService;
        }

        public void UpdateProfileName(string Name)
        {
            ProfileName.Text = Name;
        }

        public void UpdateGameCount(int GameCount)
        {
            GamesCount.Text = GameCount.ToString();
        }

        public void UpdateAchievementCount(int Achievements)
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
            if (_steamService.IsLogged())
            {
                infoStackPanel.Visibility = Visibility.Visible;
                UpdateProfileName(_steamService.Profile.SteamID);
                UpdateGameCount(_steamService.GamesList.Games.Game.Count);
                UpdateAchievementCount(_gameAchievementsService.GetTotalAchievementsCount());
                UpdateAchieved(_gameAchievementsService.GetCompletedAchievementsCount());
                UpdateAchievementStatistics(_gameAchievementsService.GetCompletedAchievementsCount() * 100 / _gameAchievementsService.GetTotalAchievementsCount());
                UpdateProgressBar(_gameAchievementsService.GetCompletedAchievementsCount() * 100 / _gameAchievementsService.GetTotalAchievementsCount());
            }
            else
                infoStackPanel.Visibility = Visibility.Collapsed;
        }
    }
}
