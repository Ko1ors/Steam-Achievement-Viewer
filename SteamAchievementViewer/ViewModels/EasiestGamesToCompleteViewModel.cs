using Sav.Common.Models;
using SteamAchievementViewer.Commands;
using SteamAchievementViewer.Services;
using System.Collections.ObjectModel;
using System.Linq;

namespace SteamAchievementViewer.ViewModels
{
    public class EasiestGamesToCompleteViewModel : ViewModelBase
    {
        private readonly ISteamService _steamService;
        private readonly IGameAchievementsService _gameAchievementsService;

        private int _currentPage;
        private int _pageSize;
        private bool _isMoreGameAvailable;

        public ObservableCollection<CompletionGameComposite> CompletionGameCollection { get; set; }

        public RelayCommand ViewLoadedCommand { get; set; }

        public RelayCommand LoadMoreGamesCommand { get; set; }


        public bool IsMoreGameAvailable
        {
            get
            {
                return _isMoreGameAvailable;
            }
            set
            {
                _isMoreGameAvailable = value;
                OnPropertyChanged(nameof(IsMoreGameAvailable));
            }
        }

        public EasiestGamesToCompleteViewModel(ISteamService steamService, IGameAchievementsService gameAchievementsService)
        {
            _steamService = steamService;
            _gameAchievementsService = gameAchievementsService;

            _currentPage = 1;
            _pageSize = 50;
            CompletionGameCollection = new ObservableCollection<CompletionGameComposite>();

            ViewLoadedCommand = new RelayCommand((obj) => OnViewLoaded());
            LoadMoreGamesCommand = new RelayCommand((obj) => LoadMoreGames(), (obj) => IsMoreGameAvailable);
        }

        private void OnViewLoaded()
        {
            LoadGames();
        }

        private void LoadMoreGames()
        {
            _currentPage++;
            LoadGames();
        }

        public void LoadGames()
        {
            if (_steamService.IsLogged())
            {
                var pagedResult = _gameAchievementsService.GetPagedEasiestGamesToComplete(_currentPage, _pageSize);
                IsMoreGameAvailable = pagedResult.Count * pagedResult.Page < pagedResult.TotalCount;
                pagedResult.Items.ToList().ForEach(g => CompletionGameCollection.Add(g));
            }
        }
    }
}
