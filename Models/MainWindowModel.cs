using System.Collections.Generic;

namespace SteamAchievementViewer.Models
{
    public class MainWindowModel : BaseModel
    {
        public List<NavigationPageElement> NavigationPages { get; set; }

        private bool _isNavigationAvailable;

        public bool IsNavigationAvailable
        {
            get { return _isNavigationAvailable; }
            set
            {
                _isNavigationAvailable = value;
                OnPropertyChanged(nameof(IsNavigationAvailable));
            }
        }
    }
}
