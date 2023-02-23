using SteamAchievementViewer.Services;
using System.Windows.Controls;

namespace SteamAchievementViewer.Pages
{
    /// <summary>
    /// Логика взаимодействия для CloseAllAchievements.xaml
    /// </summary>
    public partial class CloseAllAchievements : Page
    {
        private readonly ISteamService _steamService;
        private readonly IGameAchievementsService _gameAchievementsService;

        public CloseAllAchievements(ISteamService steamService, IGameAchievementsService gameAchievementsService)
        {
            InitializeComponent();
            _steamService = steamService;
            _gameAchievementsService = gameAchievementsService;
        }

        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_steamService.IsLogged())
                AchievementTable.ItemsSource = _gameAchievementsService.GetClosestAchievements();
        }
    }
}
