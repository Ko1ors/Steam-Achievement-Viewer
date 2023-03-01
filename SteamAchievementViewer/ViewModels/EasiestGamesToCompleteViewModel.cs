﻿using Sav.Common.Models;
using SteamAchievementViewer.Commands;
using SteamAchievementViewer.Services;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace SteamAchievementViewer.ViewModels
{
    public class EasiestGamesToCompleteViewModel : ViewModelBase
    {
        private readonly ISteamService _steamService;
        private readonly IGameAchievementsService _gameAchievementsService;

        private int _currentPage;
        private readonly int _pageSize;
        private bool _isMoreGameAvailable;
        private readonly Dispatcher _dispatcher;

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
            CompletionGameCollection = new BindingList<CompletionGameComposite>();

            ViewLoadedCommand = new RelayCommand((obj) => OnViewLoaded());
            LoadMoreGamesCommand = new RelayCommand((obj) => LoadMoreGames(), (obj) => IsMoreGameAvailable);
        }

        private void OnViewLoaded()
        {
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
            }
        }
    }
}
