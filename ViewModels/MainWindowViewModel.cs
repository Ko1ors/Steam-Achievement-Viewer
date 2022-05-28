using SteamAchievementViewer.Commands;
using SteamAchievementViewer.Models;
using SteamAchievementViewer.Services;
using System.Linq;
using System.Windows.Controls;

namespace SteamAchievementViewer.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;

        public MainWindowModel Model { get; set; }

        private Page _currentPage;

        public Page CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = value;
                OnPropertyChanged(nameof(CurrentPage));
            }
        }

        public RelayCommand NavigationCommand { get; set; }

        public MainWindowViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            _navigationService.AvailabilityChanged += NavigationService_AvailabilityChanged;
            _navigationService.NavigationChanged += NavigationService_NavigationChanged;

            NavigationCommand = new RelayCommand((obj) => Navigate(obj as NavigationPageElement), (obj) => CanNavigate());

            Model = new MainWindowModel
            {
                NavigationPages = _navigationService.GetPageElements().ToList(),
                IsNavigationAvailable = true
            };
            _navigationService.NavigateTo(Model.NavigationPages.Skip(1).First());
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
