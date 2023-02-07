using SteamAchievementViewer.Models;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace SteamAchievementViewer.Services
{
    public delegate void AvailabilityNotify(bool isAvailable);

    public delegate void NavigationNotify(Page page);

    public interface INavigationService
    {
        event AvailabilityNotify AvailabilityChanged;

        event NavigationNotify NavigationChanged;

        void AddPageElement(NavigationPageElement element);

        IEnumerable<NavigationPageElement> GetPageElements();

        void NavigateTo(Type pageType);

        void NavigateTo(NavigationPageElement element);

        void ChangeAvailability(bool isAvailable);
    }
}
