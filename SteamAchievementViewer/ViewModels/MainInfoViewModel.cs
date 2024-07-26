using Sav.Common.Logs;
using SteamAchievementViewer.Commands;
using SteamAchievementViewer.Models.SteamApi;
using SteamAchievementViewer.Services;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace SteamAchievementViewer.ViewModels
{
    public class MainInfoViewModel : ViewModelBase
    {
        private readonly ISteamService _steamService;
        private readonly IGameAchievementsService _gameAchievementsService;

        private bool _isLoaded;

        public RelayCommand ViewLoadedCommand { get; set; }

        private string _profileName;

        public string ProfileName
        {
            get
            {
                return _profileName;
            }
            set
            {
                _profileName = value;
                OnPropertyChanged(nameof(ProfileName));
            }
        }

        private int _gameCount;

        public int GameCount
        {
            get
            {
                return _gameCount;
            }
            set
            {
                _gameCount = value;
                OnPropertyChanged(nameof(GameCount));
            }
        }


        private int _totalAchievementCount;

        public int TotalAchievementCount
        {
            get
            {
                return _totalAchievementCount;
            }
            set
            {
                _totalAchievementCount = value;
                OnPropertyChanged(nameof(TotalAchievementCount));
                OnPropertyChanged(nameof(AchievementPercentage));
            }
        }

        private int _achievementCount;

        public int AchievementCount
        {
            get
            {
                return _achievementCount;
            }
            set
            {
                _achievementCount = value;
                OnPropertyChanged(nameof(AchievementCount));
                OnPropertyChanged(nameof(AchievementPercentage));
            }
        }

        public int AchievementPercentage
        {
            get
            {
                if (TotalAchievementCount == 0)
                    return 0;
                return (int)((double)AchievementCount / TotalAchievementCount * 100);
            }
        }

        private bool _loadingData;

        public bool LoadingData
        {
            get
            {
                return _loadingData;
            }
            set
            {
                _loadingData = value;
                OnPropertyChanged(nameof(LoadingData));
            }
        }

        public MainInfoViewModel(ISteamService steamService, IGameAchievementsService gameAchievementsService)
        {
            _steamService = steamService;
            _gameAchievementsService = gameAchievementsService;

            ViewLoadedCommand = new RelayCommand((obj) => OnViewLoaded(), (obj) => !_isLoaded);
        }

        private void OnViewLoaded()
        {
            LoadingData = true;
            _isLoaded = true;
            Task.Run(() => LoadInfoAsync());
        }

        public async Task LoadInfoAsync()
        {
            Log.Logger.Information("Loading main info");
            if (!_steamService.IsLogged())
                return;

            var user = _steamService.GetUser();
            if (user is null)
                return;

            ProfileName = user.SteamID;
            GameCount = _steamService.GetUserGames().Count();
            TotalAchievementCount = _gameAchievementsService.GetTotalAchievementsCount();
            AchievementCount = _gameAchievementsService.GetCompletedAchievementsCount();
            LoadingData = false;
            Log.Logger.Information("Main info loaded name: {0}, game count: {1}, total achievements: {2}, completed achievements: {3}", ProfileName, GameCount, TotalAchievementCount, AchievementCount);
        }
    }

}
