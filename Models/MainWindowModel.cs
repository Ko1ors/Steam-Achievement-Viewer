using System;
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

    public class NavigationPageElement : BaseModel
    {
        public Type Type { get; set; }

        public string Title { get; set; }

        private bool _selected;

        public bool Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                OnPropertyChanged(nameof(Selected));
            }
        }
    }
}
