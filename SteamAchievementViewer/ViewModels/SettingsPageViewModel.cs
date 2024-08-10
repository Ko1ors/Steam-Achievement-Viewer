using SteamAchievementViewer.Commands;
using System;
using System.Diagnostics;
using System.Windows.Input;

namespace SteamAchievementViewer.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        private const string HyperlinkUrl = "https://steamcommunity.com/dev/apikey";

        private string _steamID;
        private string _apiKey;
        private int _autoUpdateIntervalHours;
        private int _autoUpdateIntervalMinutes;
        private int _updateType;

        public string SteamID
        {
            get => _steamID;
            set
            {
                if (_steamID != value)
                {
                    _steamID = value;
                    OnPropertyChanged(nameof(SteamID));
                }
            }
        }

        public string APIKey
        {
            get => _apiKey;
            set
            {
                if (_apiKey != value)
                {
                    _apiKey = value;
                    OnPropertyChanged(nameof(APIKey));
                }
            }
        }

        public int AutoUpdateIntervalHours
        {
            get => _autoUpdateIntervalHours;
            set
            {
                if (_autoUpdateIntervalHours != value)
                {
                    _autoUpdateIntervalHours = value;
                    OnPropertyChanged(nameof(AutoUpdateIntervalHours));
                }
            }
        }

        public int AutoUpdateIntervalMinutes
        {
            get => _autoUpdateIntervalMinutes;
            set
            {
                if (_autoUpdateIntervalMinutes != value)
                {
                    _autoUpdateIntervalMinutes = value;
                    OnPropertyChanged(nameof(AutoUpdateIntervalMinutes));
                }
            }
        }

        public int UpdateType
        {
            get => _updateType;
            set
            {
                if (_updateType != value)
                {
                    _updateType = value;
                    OnPropertyChanged(nameof(UpdateType));
                }
            }
        }

        public ICommand AuthCommand { get; }

        public ICommand HyperlinkCommand { get; }

        public SettingsPageViewModel()
        {
            SteamID = Settings.Default.SteamID;
            APIKey = Settings.Default.SteamApiKey;
            var updateInterval = Settings.Default.UpdateInterval;
            AutoUpdateIntervalHours = updateInterval.Hours;
            AutoUpdateIntervalMinutes = updateInterval.Minutes;
            UpdateType = Settings.Default.UpdateOnlyRecentGames ? 1 : 0;
            AuthCommand = new RelayCommand(SaveSettings);
            HyperlinkCommand = new RelayCommand((obj) => Process.Start(new ProcessStartInfo("cmd", $"/c start {HyperlinkUrl}") { CreateNoWindow = true }));
        }

        private void SaveSettings(object parameter)
        {
            var updateTimeSpan = new TimeSpan(AutoUpdateIntervalHours, AutoUpdateIntervalMinutes, 0).Duration();
            if (updateTimeSpan.TotalMinutes < 1)
                updateTimeSpan = new TimeSpan(1, 0, 0);
            Settings.Default.UpdateInterval = updateTimeSpan;
            Settings.Default.SteamID = SteamID;
            Settings.Default.SteamApiKey = APIKey;
            Settings.Default.UpdateOnlyRecentGames = UpdateType == 1;
            Settings.Default.Save();
        }
    }
}
