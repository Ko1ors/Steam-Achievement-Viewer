using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace SteamAchievementViewer.Models
{
    public class MainWindowModel : BaseModel
    {
        public List<NavigationPageElement> NavigationPages { get; set; }

        private bool _isNavigationAvailable;
        private Page _currentPage;
        private string _avatarSource;
        private string _frameSource;

        public bool IsNavigationAvailable
        {
            get { return _isNavigationAvailable; }
            set
            {
                _isNavigationAvailable = value;
                OnPropertyChanged(nameof(IsNavigationAvailable));
            }
        }

        public Page CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = value;
                OnPropertyChanged(nameof(CurrentPage));
            }
        }

        public string AvatarSource
        {
            get { return _avatarSource; }
            set
            {
                _avatarSource = value;
                OnPropertyChanged(nameof(AvatarSource));
            }
        }


        public string FrameSource
        {
            get { return _frameSource; }
            set
            {
                _frameSource = value;
                OnPropertyChanged(nameof(FrameSource));
                OnPropertyChanged(nameof(IsFrameAvailable));
                OnPropertyChanged(nameof(FrameVisibility));
                OnPropertyChanged(nameof(AvatarBorderThickness));
                OnPropertyChanged(nameof(AvatarImageSize));
            }
        }

        public bool IsFrameAvailable => !string.IsNullOrEmpty(FrameSource);

        public Visibility FrameVisibility => IsFrameAvailable ? Visibility.Visible : Visibility.Collapsed;

        public int AvatarBorderThickness => IsFrameAvailable ? 0 : 3;

        public string AvatarImageSize => IsFrameAvailable ? "41" : "Auto";
    }
}
