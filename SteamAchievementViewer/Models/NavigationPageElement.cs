using System;

namespace SteamAchievementViewer.Models
{
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
