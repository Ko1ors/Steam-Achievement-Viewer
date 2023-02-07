using SteamAchievementViewer.Services;
using System.Windows;
using System.Windows.Controls;

namespace SteamAchievementViewer.Pages
{
    /// <summary>
    /// Логика взаимодействия для RareAchievements.xaml
    /// </summary>
    public partial class RareAchievements : Page
    {
        private readonly ISteamService _steamService;
        private readonly IGameAchievementsService _gameAchievementsService;

        public RareAchievements(ISteamService steamService, IGameAchievementsService gameAchievementsService)
        {
            InitializeComponent();
            _steamService = steamService;
            _gameAchievementsService = gameAchievementsService;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (_steamService.IsLogged())
                AchievementTable.ItemsSource = _gameAchievementsService.GetRarestAchievements();
        }
    }
}
