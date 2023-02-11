﻿using SteamAchievementViewer.Commands;
using SteamAchievementViewer.Services;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SteamAchievementViewer.ViewModels
{
    public class AuthPageViewModel : ViewModelBase
    {
        private const string HyperlinkUrl = "https://github.com/Ko1ors/Steam-Achievement-Viewer/blob/master/README.md#login";
        private readonly INavigationService _navigationService;
        private readonly ISteamService _steamService;

        private bool _getInformationInProcess;
        private string _statusLabelContent;
        private int _progressBarValue;
        private string _steamId;

        public string StatusLabelContent
        {
            get { return _statusLabelContent; }
            set
            {
                _statusLabelContent = value;
                OnPropertyChanged(nameof(StatusLabelContent));
            }
        }

        public int ProgressBarValue
        {
            get { return _progressBarValue; }
            set
            {
                _progressBarValue = value;
                OnPropertyChanged(nameof(ProgressBarValue));
            }
        }

        public string SteamId
        {
            get { return _steamId; }
            set
            {
                _steamId = value;
                OnPropertyChanged(nameof(SteamId));
            }
        }

        public RelayCommand AuthCommand { get; set; }

        public RelayCommand HyperlinkCommand { get; set; }

        public AuthPageViewModel(INavigationService navigationService, ISteamService steamService)
        {
            _navigationService = navigationService;
            _steamService = steamService;
            _steamService.OnAchievementProgressUpdated += SteamServiceOnAchievementProgressUpdated;

            AuthCommand = new RelayCommand((obj) => _ = GetUserInformationAsync(), (obj) => CanAuth());
            HyperlinkCommand = new RelayCommand((obj) => Process.Start(new ProcessStartInfo("cmd", $"/c start {HyperlinkUrl}") { CreateNoWindow = true }));
        }

        private bool CanAuth()
        {
            return !_getInformationInProcess && !string.IsNullOrWhiteSpace(SteamId);
        }

        private async Task GetUserInformationAsync()
        {
            try
            {
                await Task.Run(async () =>
                {
                    _getInformationInProcess = true;
                    ProgressBarValue = 0;
                    _navigationService.ChangeAvailability(false);
                    string steamID = SteamId;

                    StatusLabelContent = Properties.Resources.RetrievingProfileData;
                    await Task.Delay(1000);

                    if (await _steamService.UpdateProfileAsync(steamID))
                    {
                        StatusLabelContent = Properties.Resources.ProfileDataRetrieved;
                    }
                    else
                    {
                        StatusLabelContent = Properties.Resources.ProfileDataFailed;
                        return;
                    }

                    await Task.Delay(1000);
                    StatusLabelContent = Properties.Resources.RetrievingGameList;
                    await Task.Delay(1000);

                    if (await _steamService.UpdateGamesAsync(steamID))
                    {
                        StatusLabelContent = Properties.Resources.GameListRetrieved;
                    }
                    else
                    {
                        StatusLabelContent = Properties.Resources.GameListFailed;
                        return;
                    }

                    await Task.Delay(1000);
                    StatusLabelContent = Properties.Resources.RetrievingAchievementList;

                    await Task.Delay(1000);
                    StatusLabelContent = Properties.Resources.ResultSaving;
                    
                    _steamService.SaveSettingsInfo();

                    await Task.Delay(1000);
                    StatusLabelContent = Properties.Resources.ResultSaved;

                });
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            _navigationService.ChangeAvailability(true);
            _getInformationInProcess = false;
        }

        private void SteamServiceOnAchievementProgressUpdated(int totalGames, int currentGameCount, string lastGameName)
        {
            StatusLabelContent = Properties.Resources.RetrievingAchievementList + " " + currentGameCount + "/" + totalGames + "\t" + lastGameName;
            ProgressBarValue = currentGameCount * 100 / totalGames;
        }
    }
}
