using Sav.Common.Models;
using SteamAchievementViewer.Commands;
using SteamAchievementViewer.Services;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace SteamAchievementViewer.ViewModels
{
    public class CompletedGamesViewModel : ViewModelBase
    {
        private readonly ISteamService _steamService;
        private readonly IGameAchievementsService _gameAchievementsService;

        private readonly int _pageSize;
        private readonly Dispatcher _dispatcher;
        private int _currentPage;
        private bool _isMoreGameAvailable;
        private bool _isLoaded;

        public BindingList<CompletedGameComposite> CompletedGameCompositeCollection { get; set; }

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

        public CompletedGamesViewModel(ISteamService steamService, IGameAchievementsService gameAchievementsService)
        {
            _steamService = steamService;
            _gameAchievementsService = gameAchievementsService;

            _currentPage = 1;
            _pageSize = 50;
            _dispatcher = Dispatcher.CurrentDispatcher;
            _isLoaded = false;
            CompletedGameCompositeCollection = new BindingList<CompletedGameComposite>();

            ViewLoadedCommand = new RelayCommand((obj) => OnViewLoaded(), (obj) => !_isLoaded);
            LoadMoreGamesCommand = new RelayCommand((obj) => LoadMoreGames(), (obj) => IsMoreGameAvailable);
        }

        private void OnViewLoaded()
        {
            _isLoaded = true;
            Task.Run(() => LoadGamesAsync());
        }

        private void LoadMoreGames()
        {
            IsMoreGameAvailable = false;
            _currentPage++;
            Task.Run(() => LoadGamesAsync());
        }

        public async Task LoadGamesAsync()
        {
            if (_steamService.IsLogged())
            {
                CompletedGameCompositeCollection.RaiseListChangedEvents = false;

                var pagedResult = await _gameAchievementsService.GetPagedCompletedGamesAsync(_currentPage, _pageSize);
                IsMoreGameAvailable = _pageSize * pagedResult.Page < pagedResult.TotalCount;
                var list = pagedResult.Items;

                foreach (var game in list)
                {
                    CompletedGameCompositeCollection.Add(game);
                }

                CompletedGameCompositeCollection.RaiseListChangedEvents = true;
                _dispatcher.Invoke(CompletedGameCompositeCollection.ResetBindings, DispatcherPriority.Render);
            }
        }
    }
}
