using SteamAchievementViewer.Commands;
using SteamAchievementViewer.Models;
using SteamAchievementViewer.Services;
using System;
using System.Linq;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace SteamAchievementViewer.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly ISteamService _steamService;
        private readonly SynchronizationContext _synchronizationContext;

        private Page _currentPage;
        private BitmapImage _avatarSource;

        public MainWindowModel Model { get; set; }

        public Page CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = value;
                OnPropertyChanged(nameof(CurrentPage));
            }
        }

        public BitmapImage AvatarSource
        {
            get { return _avatarSource; }
            set
            {
                _avatarSource = value;
                OnPropertyChanged(nameof(AvatarSource));
            }
        }

        public RelayCommand NavigationCommand { get; set; }

        public MainWindowViewModel(INavigationService navigationService, ISteamService steamService)
        {
            _navigationService = navigationService;
            _steamService = steamService;
            _synchronizationContext = SynchronizationContext.Current;

            _navigationService.AvailabilityChanged += NavigationService_AvailabilityChanged;
            _navigationService.NavigationChanged += NavigationService_NavigationChanged;

            _steamService.OnAvatarUpdated += SteamServiceOnAvatarUpdated;

            NavigationCommand = new RelayCommand((obj) => Navigate(obj as NavigationPageElement), (obj) => CanNavigate());

            Model = new MainWindowModel
            {
                NavigationPages = _navigationService.GetPageElements().ToList(),
                IsNavigationAvailable = true
            };

            _steamService.Start();

            if (steamService.Profile != null)
                UpdateAvatar(steamService.Profile.AvatarFull);

            _navigationService.NavigateTo(Model.NavigationPages.Skip(1).First());
        }

        private void SteamServiceOnAvatarUpdated(string avatarUrl)
        {
            _synchronizationContext.Post((avatarUrl) => UpdateAvatar(avatarUrl.ToString()), avatarUrl);
        }

        private void UpdateAvatar(string avatarUrl)
        {
            AvatarSource = new BitmapImage(new Uri(avatarUrl));
        }

        private void NavigationService_NavigationChanged(Page page)
        {
            CurrentPage = page;
        }

        private void NavigationService_AvailabilityChanged(bool isAvailable)
        {
            Model.IsNavigationAvailable = isAvailable;
        }

        private bool CanNavigate()
        {
            return Model.IsNavigationAvailable;
        }

        private void Navigate(NavigationPageElement element)
        {
            _navigationService.NavigateTo(element);
        }
    }
}
