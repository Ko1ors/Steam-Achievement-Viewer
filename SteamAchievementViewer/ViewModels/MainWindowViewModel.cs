using Sav.Common.Logs;
using SteamAchievementViewer.Commands;
using SteamAchievementViewer.Models;
using SteamAchievementViewer.Pages;
using SteamAchievementViewer.Services;
using System.Linq;
using System.Threading;
using System.Windows.Controls;

namespace SteamAchievementViewer.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly ISteamService _steamService;
        private readonly SynchronizationContext _synchronizationContext;

        public MainWindowModel Model { get; set; }

        public RelayCommand NavigationCommand { get; set; }

        public MainWindowViewModel(INavigationService navigationService, ISteamService steamService)
        {
            _navigationService = navigationService;
            _steamService = steamService;
            _synchronizationContext = SynchronizationContext.Current;

            _navigationService.AvailabilityChanged += NavigationService_AvailabilityChanged;
            _navigationService.NavigationChanged += NavigationService_NavigationChanged;

            _steamService.OnAvatarUpdated += SteamServiceOnAvatarUpdated;

            NavigationCommand = new RelayCommand((obj) => Navigate(obj as NavigationPageElement), (obj) => CanNavigate(obj as NavigationPageElement));

            Model = new MainWindowModel
            {
                NavigationPages = _navigationService.GetPageElements().ToList(),
                IsNavigationAvailable = true
            };

            if ((steamService.GetUser() is var user) && user != null)
                UpdateAvatar(user.AvatarFull, user.AvatarFrame);

            _navigationService.NavigateTo(Model.NavigationPages.Skip(1).First());
        }

        private void SteamServiceOnAvatarUpdated(AvatarModel model)
        {
            _synchronizationContext.Post((avatarUrl) => UpdateAvatar(model.AvatarUrl, model.FrameUrl), model);
        }

        private void UpdateAvatar(string avatarUrl, string frameUrl)
        {
            Model.AvatarSource = avatarUrl;
            Model.FrameSource = frameUrl;
            Log.Logger.Information("Avatar updated, avatarUrl: {avatarUrl}, frameUrl: {frameUrl}", avatarUrl, frameUrl);
        }

        private void NavigationService_NavigationChanged(Page page)
        {
            Model.CurrentPage = page;
            Log.Logger.Information("Navigation changed to {PageType}", page.GetType().Name);
        }

        private void NavigationService_AvailabilityChanged(bool isAvailable)
        {
            Model.IsNavigationAvailable = isAvailable;
            Log.Logger.Information("Navigation availability changed to {IsAvailable}", isAvailable);
        }

        private bool CanNavigate(NavigationPageElement element)
        {
            return Model.IsNavigationAvailable || element.Type == typeof(SettingsPage);
        }

        private void Navigate(NavigationPageElement element)
        {
            _navigationService.NavigateTo(element);
        }
    }
}
