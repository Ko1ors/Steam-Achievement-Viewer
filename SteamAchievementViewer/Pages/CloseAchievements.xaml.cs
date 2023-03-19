﻿using Sav.Infrastructure.Entities;
using SteamAchievementViewer.Services;
using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;

namespace SteamAchievementViewer.Pages
{

    public partial class CloseAchievements : Page
    {
        private readonly ISteamService _steamService;
        private readonly IGameAchievementsService _gameAchievementsService;

        public CloseAchievements(ISteamService steamService, IGameAchievementsService gameAchievementsService)
        {
            InitializeComponent();
            _steamService = steamService;
            _gameAchievementsService = gameAchievementsService;
        }

        private void GameSelected(object sender, SelectionChangedEventArgs e)
        {
            if (GameList.SelectedItem != null)
            {
                AchievementList.ItemsSource = _gameAchievementsService.GetGameClosestAchievements((GameList.SelectedItem as GameEntity).AppID);
                if (AchievementList.Items.Count > 0)
                    AchievementList.ScrollIntoView(AchievementList.Items[0]);
            }
            else
                AchievementList.ItemsSource = null;
        }

        private bool GameSearchFilter(object item)
        {
            if (string.IsNullOrEmpty(textBoxSearch.Text))
                return true;
            else
                return (item as GameEntity).Name.Contains(textBoxSearch.Text, StringComparison.OrdinalIgnoreCase);
        }

        private void textBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(GameList.ItemsSource)?.Refresh();
        }

        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_steamService.IsLogged())
            {
                GameList.ItemsSource = _gameAchievementsService.GetIncompleteGames(1, int.MaxValue).ToList();
                ListCollectionView view = (ListCollectionView)CollectionViewSource.GetDefaultView(GameList.ItemsSource);
                view.Filter = GameSearchFilter;
            }
        }
    }
}
