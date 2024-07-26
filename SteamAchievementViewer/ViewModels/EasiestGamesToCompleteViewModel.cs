using Sav.Common.Logs;
using Sav.Common.Models;
using SteamAchievementViewer.Commands;
using SteamAchievementViewer.Services;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace SteamAchievementViewer.ViewModels
{
    public class EasiestGamesToCompleteViewModel : ViewModelBase
    {
        private readonly ISteamService _steamService;
        private readonly IGameAchievementsService _gameAchievementsService;

        private readonly int _pageSize;
        private readonly Dispatcher _dispatcher;
        private int _currentPage;
        private bool _isMoreGameAvailable;
        private bool _isLoaded;

        public BindingList<CompletionGameComposite> CompletionGameCollection { get; set; }

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
            _dispatcher = Dispatcher.CurrentDispatcher;
            _isLoaded = false;
            CompletionGameCollection = new BindingList<CompletionGameComposite>();

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
            Log.Logger.Information("Loading games for page {0}", _currentPage);
            if (_steamService.IsLogged())
            {
                CompletionGameCollection.RaiseListChangedEvents = false;

                var pagedResult = await _gameAchievementsService.GetPagedEasiestGamesToCompleteAsync(_currentPage, _pageSize);
                IsMoreGameAvailable = pagedResult.Count * pagedResult.Page < pagedResult.TotalCount;
                var list = pagedResult.Items;

                foreach (var game in list)
                {
                    CompletionGameCollection.Add(game);
                }

                CompletionGameCollection.RaiseListChangedEvents = true;
                _dispatcher.Invoke(CompletionGameCollection.ResetBindings, DispatcherPriority.Render);
                Log.Logger.Information("Games loaded successfully, count: {0}", list.Count());
            }
        }
    }
}
